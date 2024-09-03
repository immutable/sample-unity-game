import express, {
  Express,
  Router,
  Request,
  Response,
} from 'express';
import axios from 'axios';
import cors from 'cors';
import http from 'http';
import { providers, Wallet, Contract, PopulatedTransaction, utils } from 'ethers';
import morgan from 'morgan';
import dotenv from 'dotenv';
import { config, orderbook } from '@imtbl/sdk';
import { TransactionAction, FeeValue } from '@imtbl/sdk/dist/orderbook';
import { v4 as uuidv4 } from 'uuid';

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
      throw new Error('The type must be either \'hard\' or \'soft\'');
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
        }),
    );

    console.log(`Number of transactions to send: ${transactionsToSend.length}`);

    return res.status(200).json({ transactionsToSend });
  } catch (error) {
    console.error(error);
    return res.status(400).json({ message: 'Failed to prepare listing' });
  }
});

// Mock search NFT stacks
// `market` is hardcoded
router.get('/v1/chains/imtbl-zkevm-testnet/search/stacks', async (req: Request, res: Response) => {
  try {
    const accountAddress = req.query.account_address;
    const contractAddress = req.query.contract_address;
    const pageCursor = req.query.page_cursor ?? null;
    const pageSize = req.query.page_size ?? 5;
    const traits = req.query.trait;

    let nftUrl = `https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/accounts/${accountAddress}/nfts?contract_address=${contractAddress}&page_size=${pageSize}`;
    if (pageCursor != null) {
      nftUrl += `&page_cursor=${pageCursor}`;
    }

    const nftsResponse = await axios.get(nftUrl);
    const result: any[] = [];
    for (var item of nftsResponse.data.result) {
      const stack = {
        stack_id: uuidv4(),
        chain: 'imtbl-zkevm-testnet',
        contract_address: contractAddress,
        name: item.name,
        description: item.description,
        image: item.image,
        attributes: item.attributes,
        total_count: 1,
      };

      // Hardcoded
      const market = {
        floor_listing: {
          listing_id: uuidv4(),
          price: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
            },
            amount: {
              value: '100000000000000000',
              value_in_eth: '100000000000000000',
            }
          },
          quantity: 1,
          created_at: '2022-08-16T17:43:26.991388Z',
        },
        top_bid: {
          bid_id: uuidv4(),
          price: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
            },
            amount: {
              value: '99000000000000000000',
              value_in_eth: '99000000000000000000',
            }
          },
          quantity: 1,
          created_at: '2022-08-16T17:43:26.991388Z',
        },
        last_trade: {
          trade_id: uuidv4(),
          price: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
            },
            amount: {
              value: '9750000000000000000',
              value_in_eth: '9750000000000000000',
            }
          },
          quantity: 1,
          created_at: '2022-08-16T17:43:26.991388Z',
        }
      }

      const listingResponse = await axios.get(`https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/orders/listings?sell_item_contract_address=${contractAddress}&sell_item_token_id=${item.token_id}&status=ACTIVE&sort_direction=asc&page_size=5&sort_by=buy_item_amount`);
      const listings = listingResponse.data.result.map((listing: any) => {
        return {
          listing_id: listing.id,
          price: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
            },
            amount: {
              value: listing.buy[0].amount,
              value_in_eth: '100000000000000000',
            }
          },
          token_id: item.token_id,
          quantity: 1,
        }
      });

      const notListed = [];
      if (listings.length == 0) { // Added myself, this will actually be another API call
        notListed.push({
          token_id: item.token_id,
        });
      }

      result.push({ stack, market, listings, notListed });
    }

    return res.status(200).json({ result, page: nftsResponse.data.page, });
  } catch (error) {
    console.error(error);
    return res.status(400).json({ message: 'Failed to get stacks' });
  }
});

router.get('/v1/chains/imtbl-zkevm-testnet/search/stacks/marketplace', async (req: Request, res: Response) => {
  try {
    const accountAddress = req.query.account_address;
    const contractAddress = req.query.contract_address;
    const pageCursor = req.query.page_cursor ?? null;
    const pageSize = req.query.page_size ?? 5;
    const traits = req.query.trait;

    let ordersUrl = `https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/orders/listings?sell_item_contract_address=${contractAddress}&status=ACTIVE&sort_direction=asc&page_size=5&sort_by=buy_item_amount`;
    if (pageCursor != null) {
      ordersUrl += `&page_cursor=${pageCursor}`;
    }
    console.log(`ordersUrl: ${ordersUrl}`);

    const ordersResponse = await axios.get(ordersUrl);
    const result: any[] = [];
    for (var item of ordersResponse.data.result) {
      let nftResponse = await axios.get(`https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/collections/${contractAddress}/nfts/${item.sell[0].token_id}`);
      let nft = nftResponse.data.result;
      const stack = {
        stack_id: uuidv4(),
        chain: 'imtbl-zkevm-testnet',
        contract_address: contractAddress,
        name: nft.name,
        description: nft.description,
        image: nft.image,
        attributes: nft.attributes,
        total_count: 1,
      };

      // Hardcoded
      const market = {
        floor_listing: {
          listing_id: uuidv4(),
          price: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
            },
            amount: {
              value: '100000000000000000',
              value_in_eth: '100000000000000000',
            }
          },
          quantity: 1,
          created_at: '2022-08-16T17:43:26.991388Z',
        },
        top_bid: {
          bid_id: uuidv4(),
          price: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
            },
            amount: {
              value: '99000000000000000000',
              value_in_eth: '99000000000000000000',
            }
          },
          quantity: 1,
          created_at: '2022-08-16T17:43:26.991388Z',
        },
        last_trade: {
          trade_id: uuidv4(),
          price: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
            },
            amount: {
              value: '9750000000000000000',
              value_in_eth: '9750000000000000000',
            }
          },
          quantity: 1,
          created_at: '2022-08-16T17:43:26.991388Z',
        }
      }

      const listings = [
        {
          listing_id: item.id,
          account_address: item.account_address,
          price: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
            },
            amount: {
              value: item.buy[0].amount,
              value_in_eth: '100000000000000000',
            }
          },
          token_id: item.sell[0].token_id,
          fees: item.fees,
          quantity: 1,
        }
      ];

      result.push({ stack, market, listings, });
    }

    return res.status(200).json({ result, page: ordersResponse.data.page, });
  } catch (error) {
    console.error(error);
    return res.status(400).json({ message: 'Failed to get stacks' });
  }
});

app.use('/', router);

http.createServer(app).listen(
  6060,
  () => console.log('Listening on port 6060'),
);