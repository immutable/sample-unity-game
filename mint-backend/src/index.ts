import express, {
  Express,
  Router,
  Request,
  Response,
} from 'express';
import cors from 'cors';
import http from 'http';
import { Wallet } from 'ethers';
import { arrayify } from 'ethers/lib/utils';
import { getDefaultProvider } from '@ethersproject/providers';
import { keccak256 } from '@ethersproject/keccak256';
import { toUtf8Bytes } from '@ethersproject/strings';
import morgan from 'morgan';
import dotenv from 'dotenv';
import { x, config } from '@imtbl/sdk';

dotenv.config();

const app: Express = express();
app.use(morgan('dev')); // Logging
app.use(express.urlencoded({ extended: false })); // Parse request
app.use(express.json()); // Handle JSON
app.use(cors()); // Enable CORS
const router: Router = express.Router();

// Contract addresses
const foxContractAddress = process.env.FOX_CONTRACT_ADDRESS;

// Private key of wallet with minter role
const privateKey = process.env.PRIVATE_KEY;

// Mint Immutable Runner Fox
router.post('/mint/fox', async (req: Request, res: Response) => {
  if (!foxContractAddress || !privateKey) {
    res.writeHead(500);
    res.end();
    return;
  }

  try {
    // Set up IMXClient
    const client = new x.IMXClient(
      x.imxClientConfig({ environment: config.Environment.SANDBOX })
    );

    // Set up signer
    const provider = getDefaultProvider('sepolia');

    // Connect to wallet with minter role
    const ethSigner = new Wallet(privateKey, provider);

    const tokenId = await nextTokenId(foxContractAddress, client);
    console.log('Next token ID: ', tokenId);

    // recipient
    const recipient: string = req.body.to ?? null;

    // Set up request
    let mintRequest = {
      auth_signature: '', // This will be filled in later
      contract_address: foxContractAddress,
      users: [
        {
          user: ethSigner.address,
          tokens: [
            {
              id: tokenId.toString(),
              blueprint: 'onchain-metadata',
              royalties: [
                {
                  recipient: ethSigner.address,
                  percentage: 1,
                },
              ],
            },
          ],
        },
      ],
    };
    const message = keccak256(toUtf8Bytes(JSON.stringify(mintRequest)));
    const authSignature = await ethSigner.signMessage(arrayify(message));
    mintRequest.auth_signature = authSignature;

    console.log('sender', ethSigner.address, 'recipient', recipient, 'tokenId', tokenId);

    // Mint
    const mintResponse = await client.mint(ethSigner, mintRequest);
    console.log('Mint response: ', mintResponse);

    try {
      // Transfer to recipient
      const imxProviderConfig = new x.ProviderConfiguration({
        baseConfig: {
          environment: config.Environment.SANDBOX,
        },
      });
      const starkPrivateKey = await x.generateLegacyStarkPrivateKey(ethSigner);
      const starkSigner = x.createStarkSigner(starkPrivateKey);
      const imxProvider = new x.GenericIMXProvider(
        imxProviderConfig,
        ethSigner,
        starkSigner
      );
      const result = await imxProvider.transfer({
        type: 'ERC721',
        receiver: recipient,
        tokenAddress: foxContractAddress,
        tokenId: mintResponse.results[0].token_id,
      });
      console.log('Transfer result: ', result);

      res.writeHead(200);
      res.end(JSON.stringify(mintResponse.results[0]));
    } catch (error) {
      console.log(error);
      res.writeHead(400);
      res.end(JSON.stringify({ message: 'Failed to transfer to user' }));
    }
  } catch (error) {
    console.log(error);
    res.writeHead(400);
    res.end(JSON.stringify({ message: 'Failed to mint to user' }));
  }
});

app.use('/', router);

http.createServer(app).listen(
  3000,
  () => console.log('Listening on port 3000'),
);

/**
 * Helper function to get the next token id for a collection
 */
export const nextTokenId = async (
  collectionAddress: string,
  imxClient: x.IMXClient
) => {
  try {
    let remaining = 0;
    let cursor: string | undefined;
    let tokenId = 0;

    do {
      // eslint-disable-next-line no-await-in-loop
      const assets = await imxClient.listAssets({
        collection: collectionAddress,
        cursor,
      });
      remaining = assets.remaining;
      cursor = assets.cursor;

      for (const asset of assets.result) {
        const id = parseInt(asset.token_id, 10);
        if (id > tokenId) {
          tokenId = id;
        }
      }
    } while (remaining > 0);

    return tokenId + 1;
  } catch (error) {
    return 0;
  }
};