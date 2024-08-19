import express, {
  Express,
  Router,
  Request,
  Response,
} from 'express';
import cors from 'cors';
import http from 'http';
import { providers, Wallet, Contract, PopulatedTransaction, TypedDataDomain } from 'ethers';
import morgan from 'morgan';
import dotenv from 'dotenv';
import { config, orderbook, blockchainData } from '@imtbl/sdk';
import { PrepareListingResponse, SignableAction, FeeValue } from '@imtbl/sdk/dist/orderbook';

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
const skinContractAddress = process.env.SKIN_CONTRACT_ADDRESS;
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

const client = new orderbook.Orderbook({
  baseConfig: {
    environment: config.Environment.SANDBOX,
    publishableKey: "pk_imapik-test-DKZd2qi8Ta9JUSZoySQQ",
  },
});

const prepareERC721Listing = async (
  offererAddress: string,
  amountToSell: string,
  tokenId: string,
  // client: orderbook.Orderbook,
  // signer: Wallet,
  // provider: Provider,
): Promise<{ preparedListing: orderbook.PrepareListingResponse, transactionToSend: PopulatedTransaction | undefined, toSign: string | undefined }> => {
  if (!skinContractAddress) {
    throw new Error('Immutable Runner Skin contract address not defined');
  }
  if (!tokenContractAddress) {
    throw new Error('Immutable Runner Token contract address not defined');
  }

  const preparedListing = await client.prepareListing({
    makerAddress: offererAddress,
    // ERC20 payment token
    buy: {
      amount: amountToSell,
      type: 'ERC20',
      contractAddress: tokenContractAddress,
    },
    // ERC721 sell token
    sell: {
      contractAddress: skinContractAddress,
      tokenId: tokenId,
      type: 'ERC721',
    },
  });

  let orderSignature = ''
  let transactionToSend: PopulatedTransaction | undefined;
  let toSign: string | undefined = undefined;
  for (const action of preparedListing.actions) {
    // If the user hasn't yet approved the Immutable Seaport contract to transfer assets from this
    // collection on their behalf they'll need to do so before they create an order
    if (action.type === orderbook.ActionType.TRANSACTION) {
      const builtTx = await action.buildTransaction()
      // builtTx.nonce = await signer.getTransactionCount();
      console.log(`Submitting ${action.purpose} transaction`)
      transactionToSend = builtTx;
      // await signer.sendTransaction(builtTx);
    }

    // For an order to be created (and subsequently filled), Immutable needs a valid signature for the order data.
    // This signature is stored off-chain and is later provided to any user wishing to fulfil the open order.
    // The signature only allows the order to be fulfilled if it meets the conditions specified by the user that created the listing.
    if (action.type === orderbook.ActionType.SIGNABLE) {
      toSign = JSON.stringify(action.message);
      // orderSignature = await signer._signTypedData(
      //   action.message.domain,
      //   action.message.types,
      //   action.message.value,
      // )
    }
  }

  return { preparedListing, transactionToSend, toSign }
};


// Prepare listing
router.post('/prepareListing/skin', async (req: Request, res: Response) => {
  try {
    // Get the address of the seller
    let offererAddress: string = req.body.offererAddress ?? null;
    if (!offererAddress) {
      throw Error("Missng offererAddress");
    }
    // Get the price to sell
    let amount: string = req.body.amount ?? null;
    if (!amount) {
      throw Error("Missng amount");
    }
    // Get the token ID of the skin to sell
    let tokenId: string = req.body.tokenId ?? null;
    if (!tokenId) {
      throw Error("Missng tokenId");
    }

    // Prepare listting
    let response = await prepareERC721Listing(offererAddress, amount, tokenId);

    return res.status(200).json({
      preparedListing: JSON.stringify(response.preparedListing),
      transactionToSend: response.transactionToSend,
      toSign: response.toSign
    });

  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed prepare listing' });
  }
},
);

// Create listing
router.post('/createListing/skin', async (req: Request, res: Response) => {
  try {
    // Get the order signature
    let signature: string = req.body.signature ?? null;
    if (!signature) {
      throw Error("Missng signature");
    }
    // Get prepared listing
    let preparedListingString: string = req.body.preparedListing ?? null;
    if (!preparedListingString) {
      throw Error("Missing preparedListing");
    } else {
      console.log(`preparedListing ${preparedListingString}`);
    }
    let preparedListing: orderbook.PrepareListingResponse = JSON.parse(preparedListingString);

    const order = await client.createListing({
      orderComponents: preparedListing.orderComponents,
      orderHash: preparedListing.orderHash,
      orderSignature: signature,
      makerFees: []
    });

    return res.status(200).json(order);

  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed prepare listing' });
  }
},
);

// Cancel listing
router.post('/cancelListing/skin', async (req: Request, res: Response) => {
  try {
    // Get the address of the seller
    let offererAddress: string = req.body.offererAddress ?? null;
    if (!offererAddress) {
      throw Error("Missng offererAddress");
    }
    // Get the listing id
    let listingId: string = req.body.listingId ?? null;
    if (!listingId) {
      throw Error("Missng listingId");
    }
    // Type of cancel
    let type: string = req.body.type ?? null;
    if (!type) {
      throw Error("Missing type");
    }
    if (type != 'hard' && type != 'soft') {
      throw Error(`The type can only be 'hard' or 'soft'`);
    }

    if (type == 'hard') {
      console.log("Starting hard cancel...");
      const { cancellationAction } = await client.cancelOrdersOnChain([listingId], offererAddress);
      const unsignedCancelOrderTransaction = await cancellationAction.buildTransaction();

      console.log(`unsignedCancelOrderTransaction: ${unsignedCancelOrderTransaction}`);

      return res.status(200).json(unsignedCancelOrderTransaction);
    } else if (type == 'soft') {
      const { signableAction } = await client.prepareOrderCancellations([listingId]);
      return res.status(200).json({
        toSign: JSON.stringify(signableAction.message)
      });
    }
  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed prepare listing' });
  }
},
);

// Confirm cancel listing for soft cancel
router.post('/confirmCancelListing/skin', async (req: Request, res: Response) => {
  try {
    // Get the address of the seller
    let offererAddress: string = req.body.offererAddress ?? null;
    if (!offererAddress) {
      throw Error("Missng offererAddress");
    }
    // Get the listing id
    let listingId: string = req.body.listingId ?? null;
    if (!listingId) {
      throw Error("Missng listingId");
    }
    // Signature
    let signature: string = req.body.signature ?? null;
    if (!signature) {
      throw Error("Missing signature");
    }

    const response = await client.cancelOrders([listingId], offererAddress, signature);

    console.log(`response: ${response}`);

    return res.status(200).json(response);
  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed prepare listing' });
  }
},
);

// Fill order
router.post('/fillOrder/skin', async (req: Request, res: Response) => {
  try {
    // Get the address of the seller
    let fulfillerAddress: string = req.body.fulfillerAddress ?? null;
    if (!fulfillerAddress) {
      throw Error("Missng fulfillerAddress");
    }
    // Get the listing id
    let listingId: string = req.body.listingId ?? null;
    if (!listingId) {
      throw Error("Missng listingId");
    }
    // Get fees
    let fees: string = req.body.fees ?? null;
    if (!fees) {
      throw Error("Missng fees");
    }
    const feesValue: FeeValue[] = JSON.parse(fees);

    const { actions, expiration, order } = await client.fulfillOrder(listingId, fulfillerAddress, feesValue);

    console.log(`Fulfilling listing ${order}, transaction expiry ${expiration}`);

    const transactionsToSend = [];
    for (const action of actions) {
      if (action.type === orderbook.ActionType.TRANSACTION) {
        const builtTx = await action.buildTransaction();
        console.log(`Submitting ${action.purpose} transaction`);
        console.log(`Transaction to send ${builtTx.value}`);
        transactionsToSend.push(builtTx);
      }
    }

    console.log(`Number of transactions to send ${transactionsToSend.length}`);

    return res.status(200).json({ transactionsToSend: transactionsToSend });
  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed prepare listing' });
  }
},
);


app.use('/', router);

http.createServer(app).listen(
  6060,
  () => console.log('Listening on port 6060'),
);