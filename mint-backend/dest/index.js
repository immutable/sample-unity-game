"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const axios_1 = __importDefault(require("axios"));
const cors_1 = __importDefault(require("cors"));
const http_1 = __importDefault(require("http"));
const ethers_1 = require("ethers");
const morgan_1 = __importDefault(require("morgan"));
const dotenv_1 = __importDefault(require("dotenv"));
const sdk_1 = require("@imtbl/sdk");
const uuid_1 = require("uuid");
dotenv_1.default.config();
const app = (0, express_1.default)();
app.use((0, morgan_1.default)('dev')); // Logging
app.use(express_1.default.urlencoded({ extended: false })); // Parse request
app.use(express_1.default.json()); // Handle JSON
app.use((0, cors_1.default)()); // Enable CORS
const router = express_1.default.Router();
const zkEvmProvider = new ethers_1.providers.JsonRpcProvider('https://rpc.testnet.immutable.com');
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
router.post('/mint/fox', async (req, res) => {
    try {
        if (foxContractAddress && privateKey) {
            // Get the address to mint the fox to
            let to = req.body.to ?? null;
            // Get the quantity to mint if specified, default is one
            let quantity = parseInt(req.body.quantity ?? '1');
            // Connect to wallet with minter role
            const signer = new ethers_1.Wallet(privateKey).connect(zkEvmProvider);
            // Specify the function to call
            const abi = ['function mintByQuantity(address to, uint256 quantity)'];
            // Connect contract to the signer
            const contract = new ethers_1.Contract(foxContractAddress, abi, signer);
            // Mints the number of tokens specified
            const tx = await contract.mintByQuantity(to, quantity, gasOverrides);
            await tx.wait();
            return res.status(200).json({});
        }
        else {
            return res.status(500).json({});
        }
    }
    catch (error) {
        console.log(error);
        return res.status(400).json({ message: 'Failed to mint to user' });
    }
});
// Mint Immutable Runner Token
router.post('/mint/token', async (req, res) => {
    try {
        if (tokenContractAddress && privateKey) {
            // Get the address to mint the token to
            let to = req.body.to ?? null;
            // Get the quantity to mint if specified, default is one
            let quantity = BigInt(req.body.quantity ?? '1');
            // Connect to wallet with minter role
            const signer = new ethers_1.Wallet(privateKey).connect(zkEvmProvider);
            // Specify the function to call
            const abi = ['function mint(address to, uint256 quantity)'];
            // Connect contract to the signer
            const contract = new ethers_1.Contract(tokenContractAddress, abi, signer);
            // Mints the number of tokens specified
            const tx = await contract.mint(to, quantity, gasOverrides);
            await tx.wait();
            return res.status(200).json({});
        }
        else {
            return res.status(500).json({});
        }
    }
    catch (error) {
        console.log(error);
        return res.status(400).json({ message: 'Failed to mint to user' });
    }
});
router.post('/mint/skin', async (req, res) => {
    try {
        if ('0xad826e89cde60e4ee248980d35c0f5c1196ad059' && privateKey) {
            // Get the address to mint the token to
            let to = req.body.to ?? null;
            // Get the quantity to mint if specified, default is one
            let quantity = BigInt(req.body.quantity ?? '1');
            // Connect to wallet with minter role
            const signer = new ethers_1.Wallet(privateKey).connect(zkEvmProvider);
            // Specify the function to call
            const abi = ['function mint(address to, uint256 quantity)'];
            // Connect contract to the signer
            const contract = new ethers_1.Contract('0xad826e89cde60e4ee248980d35c0f5c1196ad059', abi, signer);
            // Mints the number of tokens specified
            const tx = await contract.mint(to, quantity, gasOverrides);
            await tx.wait();
            return res.status(200).json({});
        }
        else {
            return res.status(500).json({});
        }
    }
    catch (error) {
        console.log(error);
        return res.status(400).json({ message: 'Failed to mint to user' });
    }
});
// In-game ERC20 balance
router.get('/balance', async (req, res) => {
    try {
        if (tokenContractAddress && privateKey) {
            // Get the address
            let address = req.query.address ?? null;
            // Call balanceOf
            const abi = ['function balanceOf(address account) view returns (uint256)'];
            const contract = new ethers_1.Contract(tokenContractAddress, abi, zkEvmProvider);
            const balance = await contract.balanceOf(address);
            return res.status(200).json({
                quantity: ethers_1.utils.formatUnits(balance, 18),
            });
        }
        else {
            return res.status(500).json({});
        }
    }
    catch (error) {
        console.log(error);
        return res.status(400).json({ message: 'Failed to mint to user' });
    }
});
// List item
const client = new sdk_1.orderbook.Orderbook({
    baseConfig: {
        environment: sdk_1.config.Environment.SANDBOX,
        publishableKey: process.env.PUBLISHABLE_KEY,
    },
});
const prepareListing = async (offererAddress, amountToSell, tokenId) => {
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
    let transactionToSend;
    let payloadToSign;
    for (const action of preparedListing.actions) {
        if (action.type === sdk_1.orderbook.ActionType.TRANSACTION) {
            transactionToSend = await action.buildTransaction();
        }
        if (action.type === sdk_1.orderbook.ActionType.SIGNABLE) {
            payloadToSign = JSON.stringify(action.message);
        }
    }
    return { preparedListing, transactionToSend, payloadToSign };
};
router.post('/prepareListing/skin', async (req, res) => {
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
    }
    catch (error) {
        console.error(error);
        return res.status(400).json({ message: 'Failed to prepare listing' });
    }
});
router.post('/createListing/skin', async (req, res) => {
    try {
        const { signature, preparedListing: preparedListingString } = req.body;
        if (!signature) {
            throw new Error('Missing signature');
        }
        if (!preparedListingString) {
            throw new Error('Missing preparedListing');
        }
        console.log(`Prepared Listing: ${preparedListingString}`);
        const preparedListing = JSON.parse(preparedListingString);
        const order = await client.createListing({
            orderComponents: preparedListing.orderComponents,
            orderHash: preparedListing.orderHash,
            orderSignature: signature,
            makerFees: [],
        });
        return res.status(200).json(order);
    }
    catch (error) {
        console.error(error);
        return res.status(400).json({ message: 'Failed to prepare listing' });
    }
});
// Cancel listing
router.post('/cancelListing/skin', async (req, res) => {
    try {
        const offererAddress = req.body.offererAddress;
        const listingId = req.body.listingId;
        const type = req.body.type;
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
    }
    catch (error) {
        console.error(error);
        return res.status(400).json({ message: 'Failed to prepare listing' });
    }
});
// Fulfill order
router.post('/fillOrder/skin', async (req, res) => {
    try {
        const fulfillerAddress = req.body.fulfillerAddress;
        const listingId = req.body.listingId;
        const fees = req.body.fees;
        if (!fulfillerAddress) {
            throw new Error('Missing fulfillerAddress');
        }
        if (!listingId) {
            throw new Error('Missing listingId');
        }
        if (!fees) {
            throw new Error('Missing fees');
        }
        const feesValue = JSON.parse(fees);
        const { actions, expiration, order } = await client.fulfillOrder(listingId, fulfillerAddress, feesValue);
        console.log(`Fulfilling listing ${order}, transaction expiry ${expiration}`);
        const transactionsToSend = await Promise.all(actions
            .filter((action) => action.type === sdk_1.orderbook.ActionType.TRANSACTION)
            .map(async (action) => {
            const builtTx = await action.buildTransaction();
            return builtTx;
        }));
        console.log(`Number of transactions to send: ${transactionsToSend.length}`);
        return res.status(200).json({ transactionsToSend });
    }
    catch (error) {
        console.error(error);
        return res.status(400).json({ message: 'Failed to prepare listing' });
    }
});
// Mock search NFT stacks
// `market` is hardcoded
router.get('/v1/chains/imtbl-zkevm-testnet/search/stacks', async (req, res) => {
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
        const nftsResponse = await axios_1.default.get(nftUrl);
        const result = [];
        for (var item of nftsResponse.data.result) {
            const stack = {
                stack_id: (0, uuid_1.v4)(),
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
                    listing_id: (0, uuid_1.v4)(),
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
                    bid_id: (0, uuid_1.v4)(),
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
                    trade_id: (0, uuid_1.v4)(),
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
            };
            const listingResponse = await axios_1.default.get(`https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/orders/listings?sell_item_contract_address=${contractAddress}&sell_item_token_id=${item.token_id}&status=ACTIVE&sort_direction=asc&page_size=5&sort_by=buy_item_amount`);
            const listings = listingResponse.data.result.map((listing) => {
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
                };
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
    }
    catch (error) {
        console.error(error);
        return res.status(400).json({ message: 'Failed to get stacks' });
    }
});
router.get('/v1/chains/imtbl-zkevm-testnet/search/stacks/marketplace', async (req, res) => {
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
        const ordersResponse = await axios_1.default.get(ordersUrl);
        const result = [];
        for (var item of ordersResponse.data.result) {
            let nftResponse = await axios_1.default.get(`https://api.sandbox.immutable.com/v1/chains/imtbl-zkevm-testnet/collections/${contractAddress}/nfts/${item.sell[0].token_id}`);
            let nft = nftResponse.data.result;
            const stack = {
                stack_id: (0, uuid_1.v4)(),
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
                    listing_id: (0, uuid_1.v4)(),
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
                    bid_id: (0, uuid_1.v4)(),
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
                    trade_id: (0, uuid_1.v4)(),
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
            };
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
    }
    catch (error) {
        console.error(error);
        return res.status(400).json({ message: 'Failed to get stacks' });
    }
});
app.use('/', router);
http_1.default.createServer(app).listen(6060, () => console.log('Listening on port 6060'));
