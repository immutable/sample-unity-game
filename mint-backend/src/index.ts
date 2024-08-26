import express, {
  Express,
  Router,
  Request,
  Response,
} from 'express';
import cors from 'cors';
import http from 'http';
import { providers, Wallet, Contract, PopulatedTransaction, utils } from 'ethers';
import morgan from 'morgan';
import dotenv from 'dotenv';
import { config, orderbook } from '@imtbl/sdk';
import { TransactionAction, FeeValue } from '@imtbl/sdk/dist/orderbook';

dotenv.config();

const app: Express = express();
app.use(morgan('dev')); // Logging
app.use(express.urlencoded({ extended: false })); // Parse request
app.use(express.json()); // Handle JSON
app.use(cors()); // Enable CORS
const router: Router = express.Router();

const zkEvmProvider = new providers.JsonRpcProvider('https://rpc.testnet.immutable.com');

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
      let to: string = req.body.to ?? null;
      // Get the quantity to mint if specified, default is one
      let quantity = parseInt(req.body.quantity ?? '1');

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
      let to: string = req.body.to ?? null;
      // Get the quantity to mint if specified, default is one
      let quantity = BigInt(req.body.quantity ?? '1');

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
    if ('0xad826e89cde60e4ee248980d35c0f5c1196ad059' && privateKey) {
      // Get the address to mint the token to
      let to: string = req.body.to ?? null;
      // Get the quantity to mint if specified, default is one
      let quantity = BigInt(req.body.quantity ?? '1');

      // Connect to wallet with minter role
      const signer = new Wallet(privateKey).connect(zkEvmProvider);

      // Specify the function to call
      const abi = ['function mint(address to, uint256 quantity)'];
      // Connect contract to the signer
      const contract = new Contract('0xad826e89cde60e4ee248980d35c0f5c1196ad059', abi, signer);

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

// In-game ERC20 balance
router.get('/balance', async (req: Request, res: Response) => {
  try {
    if (tokenContractAddress && privateKey) {
      // Get the address
      let address = req.query.address ?? null;

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

// List item
const client = new orderbook.Orderbook({
  baseConfig: {
    environment: config.Environment.SANDBOX,
    publishableKey: process.env.PUBLISHABLE_KEY,
  },
});

const prepareListing = async (
  offererAddress: string,
  amountToSell: string,
  tokenId: string,
): Promise<{
  preparedListing: orderbook.PrepareListingResponse;
  transactionToSend: PopulatedTransaction | undefined;
  payloadToSign: string | undefined;
}> => {
  if (!skinColourContractAddress) {
    throw new Error('Immutable Runner Skin contract address not defined');
  }
  if (!tokenContractAddress) {
    throw new Error('Immutable Runner Token contract address not defined');
  }

  const preparedListing = await client.prepareListing({
    makerAddress: offererAddress,
    buy: {
      amount: amountToSell,
      type: 'ERC20',
      contractAddress: tokenContractAddress,
    },
    sell: {
      contractAddress: skinColourContractAddress,
      tokenId,
      type: 'ERC721',
    },
  });

  let transactionToSend: PopulatedTransaction | undefined;
  let payloadToSign: string | undefined;

  for (const action of preparedListing.actions) {
    if (action.type === orderbook.ActionType.TRANSACTION) {
      transactionToSend = await action.buildTransaction();
    }

    if (action.type === orderbook.ActionType.SIGNABLE) {
      payloadToSign = JSON.stringify(action.message);
    }
  }

  return { preparedListing, transactionToSend, payloadToSign };
};

router.post('/prepareListing/skin', async (req: Request, res: Response) => {
  try {
    const { offererAddress, amount, tokenId } = req.body;

    if (!offererAddress || !amount || !tokenId) {
      throw new Error('Missing required parameters: offererAddress, amount, or tokenId');
    }

    const response = await prepareListing(offererAddress, amount, tokenId);

    return res.status(200).json({
      preparedListing: JSON.stringify(response.preparedListing),
      transactionToSend: response.transactionToSend,
      toSign: response.payloadToSign,
    });
  } catch (error) {
    console.error(error);
    return res.status(400).json({ message: 'Failed to prepare listing' });
  }
});

router.post('/createListing/skin', async (req: Request, res: Response) => {
  try {
    const { signature, preparedListing: preparedListingString } = req.body;

    if (!signature) {
      throw new Error('Missing signature');
    }

    if (!preparedListingString) {
      throw new Error('Missing preparedListing');
    }

    console.log(`Prepared Listing: ${preparedListingString}`);
    const preparedListing: orderbook.PrepareListingResponse = JSON.parse(preparedListingString);

    const order = await client.createListing({
      orderComponents: preparedListing.orderComponents,
      orderHash: preparedListing.orderHash,
      orderSignature: signature,
      makerFees: []
    });

    return res.status(200).json(order);

  } catch (error) {
    console.error(error);
    return res.status(400).json({ message: 'Failed to prepare listing' });
  }
});

// Cancel listing
router.post('/cancelListing/skin', async (req: Request, res: Response) => {
  try {
    const offererAddress: string = req.body.offererAddress;
    const listingId: string = req.body.listingId;
    const type: string = req.body.type;

    if (!offererAddress) {
      throw new Error('Missing offererAddress');
    }
    if (!listingId) {
      throw new Error('Missing listingId');
    }
    if (!type || (type !== 'hard' && type !== 'soft')) {
      throw new Error(`The type must be either 'hard' or 'soft'`);
    }

    if (type === 'hard') {
      const { cancellationAction } = await client.cancelOrdersOnChain([listingId], offererAddress);
      const unsignedCancelOrderTransaction = await cancellationAction.buildTransaction();

      console.log(`unsignedCancelOrderTransaction: ${unsignedCancelOrderTransaction}`);

      return res.status(200).json(unsignedCancelOrderTransaction);
    }

    if (type === 'soft') {
      const { signableAction } = await client.prepareOrderCancellations([listingId]);
      return res.status(200).json({
        toSign: JSON.stringify(signableAction.message),
      });
    }
  } catch (error) {
    console.error(error);
    return res.status(400).json({ message: 'Failed to prepare listing' });
  }
});

// Fulfill order
router.post('/fillOrder/skin', async (req: Request, res: Response) => {
  try {
    const fulfillerAddress: string = req.body.fulfillerAddress;
    const listingId: string = req.body.listingId;
    const fees: string = req.body.fees;

    if (!fulfillerAddress) {
      throw new Error('Missing fulfillerAddress');
    }
    if (!listingId) {
      throw new Error('Missing listingId');
    }
    if (!fees) {
      throw new Error('Missing fees');
    }

    const feesValue: FeeValue[] = JSON.parse(fees);
    const { actions, expiration, order } = await client.fulfillOrder(listingId, fulfillerAddress, feesValue);

    console.log(`Fulfilling listing ${order}, transaction expiry ${expiration}`);

    const transactionsToSend = await Promise.all(
      actions
        .filter((action): action is TransactionAction => action.type === orderbook.ActionType.TRANSACTION)
        .map(async action => {
          const builtTx = await action.buildTransaction();
          return builtTx;
        })
    );

    console.log(`Number of transactions to send: ${transactionsToSend.length}`);

    return res.status(200).json({ transactionsToSend });
  } catch (error) {
    console.error(error);
    return res.status(400).json({ message: 'Failed to prepare listing' });
  }
});

app.use('/', router);

http.createServer(app).listen(
  6060,
  () => console.log('Listening on port 6060'),
);