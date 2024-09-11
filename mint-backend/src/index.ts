import express, {
  Express,
  Router,
  Request,
  Response,
} from 'express';
import axios from 'axios';
import cors from 'cors';
import http from 'http';
import { providers, Wallet, Contract, utils } from 'ethers';
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

const apiEnv = 'dev';
const chainName = 'imtbl-zkevm-devnet';
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
    if (tokenContractAddress && privateKey) {
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

// List item
const client = new orderbook.Orderbook({
  baseConfig: {
    environment: config.Environment.SANDBOX,
    publishableKey: process.env.PUBLISHABLE_KEY,
  },
  overrides: {
    seaportContractAddress: '0xbA22c310787e9a3D74343B17AB0Ab946c28DFB52',
    zoneContractAddress: '0xb71EB38e6B51Ee7A45A632d46f17062e249580bE', // ImmutableSignedZoneV2
    apiEndpoint: 'https://api.dev.immutable.com',
    chainName: 'imtbl-zkevm-devnet',
    jsonRpcProviderUrl: 'https://rpc.dev.immutable.com'
  }
});

// Prepare listing
router.post('/v1/ts-sdk/v1/orderbook/prepareListing', async (req: Request, res: Response) => {
  try {
    const response = await client.prepareListing({
      makerAddress: req.body.makerAddress,
      buy: req.body.buy,
      sell: req.body.sell,
      orderExpiry: req.body.orderExpiry ? new Date() : undefined,
    });

    return res.status(200).json({
      actions: await Promise.all(
        response.actions.map(async (action: orderbook.Action) => {
          if (action.type === orderbook.ActionType.TRANSACTION) {
            const builtTx = await action.buildTransaction();
            return {
              populatedTransactions: builtTx,
              ...action,
            };
          } else {
            return action;
          }
        })
      ),
      orderComponents: response.orderComponents,
      orderHash: response.orderHash,
    });

  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed prepare listing' });
  }
},
);

router.post('/v1/ts-sdk/v1/orderbook/createListing', async (req: Request, res: Response) => {
  try {
    const order = await client.createListing({
      makerFees: req.body.makerFees,
      orderComponents: req.body.orderComponents,
      orderHash: req.body.orderHash,
      orderSignature: req.body.orderSignature,
    });

    console.log(`createListing: ${JSON.stringify(order)}`);
    return res.status(200).json(order);

  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed prepare listing' });
  }
},
);

router.post('/v1/ts-sdk/v1/orderbook/cancelOrdersOnChain', async (req: Request, res: Response) => {
  try {
    const { cancellationAction } = await client.cancelOrdersOnChain(req.body.orderIds, req.body.accountAddress);
    const unsignedCancelOrderTransaction = await cancellationAction.buildTransaction();

    return res.status(200).json({
      cancellationAction: {
        ...cancellationAction,
        populatedTransaction: unsignedCancelOrderTransaction
      }
    });
  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: 'Failed prepare listing' });
  }
},
);

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
router.get(`/experimental/chains/${chainName}/search/stacks`, async (req: Request, res: Response) => {
  try {
    const accountAddress = req.query.account_address;
    const contractAddress = req.query.contract_address;
    const pageCursor = req.query.page_cursor ?? null;
    const pageSize = req.query.page_size ?? 5;

    if (!accountAddress) {
      return getMarketplace(req, res);
    }

    if (!accountAddress) {
      return getMarketplace(req, res);
    }

    let nftUrl = `https://api.${apiEnv}.immutable.com/v1/chains/${chainName}/accounts/${accountAddress}/nfts?contract_address=${contractAddress}&page_size=${pageSize}`;
    console.log(`nftUrl: ${nftUrl}`);
    if (pageCursor != null) {
      nftUrl += `&page_cursor=${pageCursor}`;
    }

    const nftsResponse = await axios.get(nftUrl);
    const result = [];
    for (const item of nftsResponse.data.result) {
      const stack = {
        stack_id: uuidv4(),
        chain: { id: uuidv4(), name: chainName },
        contract_address: contractAddress,
        name: item.name,
        description: item.description,
        image: item.image,
        attributes: item.attributes?.map((a: any) => {
          return {
            display_type: a.display_type,
            trait_type: a.trait_type ?? '',
            value: a.value
          };
        }),
        total_count: 1,
        created_at: '2022-08-16T17:43:26.991388Z',
        updated_at: '2022-08-16T17:43:26.991388Z',
        external_url: '',
        animation_url: '',
        youtube_url: '',
      };

      // Hardcoded
      const market = {
        floor_listing: {
          listing_id: uuidv4(),
          creator: '',
          price_details: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
              contract_address: '0x328766302e7617d0de5901f8da139dca49f3ec75',
              decimals: '18',
            },
            amount: {
              value: '100000000000000000',
              value_in_eth: '100000000000000000',
            },
            fee_inclusive_amount: {
              value: '100000000000000000',
              value_in_eth: '100000000000000000',
            },
            fees: [
              {
                amount: '10000',
                type: 'ROYALTY',
                recipient_address: '0xaddress',
              }
            ],
          },
          token_id: '1',
          amount: '987',
        },
        last_trade: {
          trade_id: uuidv4(),
          price_details: [
            {
              token: {
                type: 'ERC20',
                symbol: 'IMR',
                contract_address: '0x328766302e7617d0de5901f8da139dca49f3ec75',
                decimals: '18',
              },
              amount: {
                value: '100000000000000000',
                value_in_eth: '100000000000000000',
              },
              fee_inclusive_amount: {
                value: '100000000000000000',
                value_in_eth: '100000000000000000',
              },
              fees: [
                {
                  amount: '10000',
                  type: 'ROYALTY',
                  recipient_address: '0xaddress',
                }
              ],
            }
          ],
          amount: '22',
          token_id: '1',
          created_at: '2022-08-16T17:43:26.991388Z',
        }
      }

      const listingResponse = await axios.get(`https://api.${apiEnv}.immutable.com/v1/chains/${chainName}/orders/listings?sell_item_contract_address=${contractAddress}&sell_item_token_id=${item.token_id}&status=ACTIVE&sort_direction=asc&page_size=5&sort_by=buy_item_amount`);
      const listings = listingResponse.data.result.map((listing) => {
        return {
          listing_id: listing.id,
          creator: accountAddress,
          price_details: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
              contract_address: '0x328766302e7617d0de5901f8da139dca49f3ec75',
              decimals: '18',
            },
            amount: {
              value: listing.buy[0].amount,
              value_in_eth: '100000000000000000',
            },
            fee_inclusive_amount: {
              value: '100000000000000000',
              value_in_eth: '100000000000000000',
            },
            fees: listing.fees,
          },
          token_id: item.token_id,
          amount: 1,
        }
      });

      const notListed = [];
      if (listings.length == 0) { // Added myself, this will actually be another API call
        notListed.push({
          listing_id: uuidv4(),
          creator: accountAddress,
          price_details: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
              contract_address: '0x328766302e7617d0de5901f8da139dca49f3ec75',
              decimals: '18',
            },
            amount: {
              value: '',
              value_in_eth: '100000000000000000',
            },
            fee_inclusive_amount: {
              value: '100000000000000000',
              value_in_eth: '100000000000000000',
            },
            fees: [
              {
                amount: '10000',
                type: 'ROYALTY',
                recipient_address: '0xaddress',
              }
            ],
          },
          token_id: item.token_id,
          amount: 1
        });
      }

      result.push({ stack, market, listings, notListed, stack_count: 1 });
    }

    return res.status(200).json({ result, page: nftsResponse.data.page, });
  } catch (error) {
    console.error(error);
    return res.status(400).json({ message: 'Failed to get stacks' });
  }
});

const getMarketplace = async (req: Request, res: Response) => {
  try {
    const contractAddress = req.query.contract_address;
    const pageCursor = req.query.page_cursor ?? null;

    let ordersUrl = `https://api.${apiEnv}.immutable.com/v1/chains/${chainName}/orders/listings?sell_item_contract_address=${contractAddress}&status=ACTIVE&sort_direction=asc&page_size=5&sort_by=buy_item_amount`;
    if (pageCursor != null) {
      ordersUrl += `&page_cursor=${pageCursor}`;
    }
    console.log(`ordersUrl: ${ordersUrl}`);

    const ordersResponse = await axios.get(ordersUrl);
    const result = [];
    for (const item of ordersResponse.data.result) {
      const nftResponse = await axios.get(`https://api.${apiEnv}.immutable.com/v1/chains/${chainName}/collections/${contractAddress}/nfts/${item.sell[0].token_id}`);
      const nft = nftResponse.data.result;
      const stack = {
        stack_id: uuidv4(),
        chain: { id: uuidv4(), name: chainName },
        contract_address: contractAddress,
        name: nft.name,
        description: nft.description,
        image: nft.image,
        attributes: nft.attributes,
        total_count: 1,
        created_at: '2022-08-16T17:43:26.991388Z',
        updated_at: '2022-08-16T17:43:26.991388Z',
        external_url: '',
        animation_url: '',
        youtube_url: '',
      };

      // Hardcoded
      const market = {
        floor_listing: {
          listing_id: uuidv4(),
          creator: '',
          price_details: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
              contract_address: '0x328766302e7617d0de5901f8da139dca49f3ec75',
              decimals: '18',
            },
            amount: {
              value: '100000000000000000',
              value_in_eth: '100000000000000000',
            },
            fee_inclusive_amount: {
              value: '100000000000000000',
              value_in_eth: '100000000000000000',
            },
            fees: [
              {
                amount: '10000',
                type: 'ROYALTY',
                recipient_address: '0xaddress',
              }
            ],
          },
          token_id: '1',
          amount: '987',
        },
        last_trade: {
          trade_id: uuidv4(),
          price_details: [
            {
              token: {
                type: 'ERC20',
                symbol: 'IMR',
                contract_address: '0x328766302e7617d0de5901f8da139dca49f3ec75',
                decimals: '18',
              },
              amount: {
                value: '100000000000000000',
                value_in_eth: '100000000000000000',
              },
              fee_inclusive_amount: {
                value: '100000000000000000',
                value_in_eth: '100000000000000000',
              },
              fees: [
                {
                  amount: '10000',
                  type: 'ROYALTY',
                  recipient_address: '0xaddress',
                }
              ],
            }
          ],
          amount: '22',
          token_id: '1',
          created_at: '2022-08-16T17:43:26.991388Z',
        }
      }

      const listings = [
        {
          listing_id: item.id,
          account_address: item.account_address,
          price_details: {
            token: {
              type: 'ERC20',
              symbol: 'IMR',
            },
            amount: {
              value: item.buy[0].amount,
              value_in_eth: '100000000000000000',
            },
            fee_inclusive_amount: {
              value: '100000000000000000',
              value_in_eth: '100000000000000000',
            },
            fees: item.fees,
          },
          token_id: item.sell[0].token_id,
          amount: 1,
        }
      ];

      result.push({ stack, market, listings });
    }

    return res.status(200).json({ result, page: ordersResponse.data.page, });
  } catch (error) {
    console.error(error);
    return res.status(400).json({ message: 'Failed to get stacks' });
  }
}

app.use('/', router);

http.createServer(app).listen(
  6060,
  () => console.log('Listening on port 6060'),
);
