import express, {
  Express,
  Router,
  Request,
  Response,
} from 'express';
import cors from 'cors';
import http from 'http';
import { providers, Wallet, Contract, utils } from 'ethers';
import morgan from 'morgan';
import dotenv from 'dotenv';

dotenv.config();

const app: Express = express();
app.use(morgan('dev')); // Logging
app.use(express.urlencoded({ extended: false })); // Parse request
app.use(express.json()); // Handle JSON
app.use(cors()); // Enable CORS
const router: Router = express.Router();

const zkEvmProvider = new providers.JsonRpcProvider(`https://rpc.dev.immutable.com`);

// Contract addresses
const foxContractAddress = process.env.FOX_CONTRACT_ADDRESS;
const tokenContractAddress = process.env.TOKEN_CONTRACT_ADDRESS;
const skinColourContractAddress = process.env.SKIN_CONTRACT_ADDRESS_COLOUR;
// Private key of wallet with minter role
const privateKey = process.env.PRIVATE_KEY;

const gasOverrides = {
  // Use parameter to set tip for EIP1559 transaction (gas fee)
  maxPriorityFeePerGas: 10e9, // 10 Gwei. This must exceed minimum gas fee expectation from the chain
  maxFeePerGas: 15e9, // 15 Gwei
};

// Mint Immutable Runner Fox
router.post('/mint/fox', async (req: Request, res: Response) => {
  try {
    if (foxContractAddress && privateKey) {
      // Get the address to mint the fox to
      const to: string = req.body.to ?? null;
      // Get the quantity to mint if specified, default is one
      const quantity = parseInt(req.body.quantity ?? '1');

      // Connect to wallet with minter role
      const signer = new Wallet(privateKey).connect(zkEvmProvider);

      // Specify the function to call
      const abi = ['function mintByQuantity(address to, uint256 quantity)'];
      // Connect contract to the signer
      const contract = new Contract(foxContractAddress, abi, signer);

      // Mints the number of tokens specified
      const tx = await contract.mintByQuantity(to, quantity, gasOverrides);
      await tx.wait();

      return res.status(200).json({});
    } else {
      return res.status(500).json({});
    }

  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed to mint to user' });
  }
},
);

// Mint Immutable Runner Token
router.post('/mint/token', async (req: Request, res: Response) => {
  try {
    if (tokenContractAddress && privateKey) {
      // Get the address to mint the token to
      const to: string = req.body.to ?? null;
      // Get the quantity to mint if specified, default is one
      const quantity = BigInt(req.body.quantity ?? '1');

      // Connect to wallet with minter role
      const signer = new Wallet(privateKey).connect(zkEvmProvider);

      // Specify the function to call
      const abi = ['function mint(address to, uint256 quantity)'];
      // Connect contract to the signer
      const contract = new Contract(tokenContractAddress, abi, signer);

      // Mints the number of tokens specified
      const tx = await contract.mint(to, quantity, gasOverrides);
      await tx.wait();

      return res.status(200).json({});
    } else {
      return res.status(500).json({});
    }

  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed to mint to user' });
  }
},
);

router.post('/mint/skin', async (req: Request, res: Response) => {
  try {
    if (skinColourContractAddress && privateKey) {
      // Get the address to mint the token to
      const to: string = req.body.to ?? null;
      // Get the quantity to mint if specified, default is one
      const tokenId = BigInt(req.body.tokenId ?? '1');

      // Connect to wallet with minter role
      const signer = new Wallet(privateKey).connect(zkEvmProvider);

      // Specify the function to call
      const abi = ['function mint(address to, uint256 tokenId)'];
      // Connect contract to the signer
      const contract = new Contract(skinColourContractAddress, abi, signer);

      // Mints the number of tokens specified
      const tx = await contract.mint(to, tokenId, gasOverrides);
      await tx.wait();

      return res.status(200).json({});
    } else {
      return res.status(500).json({});
    }

  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed to mint to user' });
  }
},
);

// In-game ERC20 balance
router.get('/balance', async (req: Request, res: Response) => {
  try {
    if (tokenContractAddress) {
      // Get the address
      const address = req.query.address ?? null;

      // Call balanceOf
      const abi = ['function balanceOf(address account) view returns (uint256)'];
      const contract = new Contract(tokenContractAddress, abi, zkEvmProvider);
      const balance = await contract.balanceOf(address);

      return res.status(200).json({
        quantity: utils.formatUnits(balance, 18),
      });
    } else {
      return res.status(500).json({});
    }

  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed to mint to user' });
  }
},
);

app.use('/', router);

http.createServer(app).listen(
  6060,
  () => console.log('Listening on port 6060'),
);
