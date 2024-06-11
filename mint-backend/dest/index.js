"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const cors_1 = __importDefault(require("cors"));
const http_1 = __importDefault(require("http"));
const ethers_1 = require("ethers");
const morgan_1 = __importDefault(require("morgan"));
const dotenv_1 = __importDefault(require("dotenv"));
const sdk_1 = require("@imtbl/sdk");
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
const skinContractAddress = process.env.SKIN_CONTRACT_ADDRESS;
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
const client = new sdk_1.orderbook.Orderbook({
    baseConfig: {
        environment: sdk_1.config.Environment.SANDBOX,
        publishableKey: "pk_imapik-test-DKZd2qi8Ta9JUSZoySQQ",
    },
});
const prepareERC721Listing = async (offererAddress, amountToSell, tokenId) => {
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
    let orderSignature = '';
    let transactionToSend;
    let toSign = undefined;
    for (const action of preparedListing.actions) {
        // If the user hasn't yet approved the Immutable Seaport contract to transfer assets from this
        // collection on their behalf they'll need to do so before they create an order
        if (action.type === sdk_1.orderbook.ActionType.TRANSACTION) {
            const builtTx = await action.buildTransaction();
            // builtTx.nonce = await signer.getTransactionCount();
            console.log(`Submitting ${action.purpose} transaction`);
            transactionToSend = builtTx;
            // await signer.sendTransaction(builtTx);
        }
        // For an order to be created (and subsequently filled), Immutable needs a valid signature for the order data.
        // This signature is stored off-chain and is later provided to any user wishing to fulfil the open order.
        // The signature only allows the order to be fulfilled if it meets the conditions specified by the user that created the listing.
        if (action.type === sdk_1.orderbook.ActionType.SIGNABLE) {
            toSign = JSON.stringify(action.message);
            // orderSignature = await signer._signTypedData(
            //   action.message.domain,
            //   action.message.types,
            //   action.message.value,
            // )
        }
    }
    return { preparedListing, transactionToSend, toSign };
};
// Prepare listing
router.post('/prepareListing/skin', async (req, res) => {
    try {
        // Get the address of the seller
        let offererAddress = req.body.offererAddress ?? null;
        if (!offererAddress) {
            throw Error("Missng offererAddress");
        }
        // Get the price to sell
        let amount = req.body.amount ?? null;
        if (!amount) {
            throw Error("Missng amount");
        }
        // Get the token ID of the skin to sell
        let tokenId = req.body.tokenId ?? null;
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
    }
    catch (error) {
        console.log(error);
        return res.status(400).json({ message: 'Failed prepare listing' });
    }
});
// Create listing
router.post('/createListing/skin', async (req, res) => {
    try {
        // Get the order signature
        let signature = req.body.signature ?? null;
        if (!signature) {
            throw Error("Missng signature");
        }
        // Get prepared listing
        let preparedListingString = req.body.preparedListing ?? null;
        if (!preparedListingString) {
            throw Error("Missing preparedListing");
        }
        else {
            console.log(`preparedListing ${preparedListingString}`);
        }
        let preparedListing = JSON.parse(preparedListingString);
        const order = await client.createListing({
            orderComponents: preparedListing.orderComponents,
            orderHash: preparedListing.orderHash,
            orderSignature: signature,
            makerFees: []
        });
        return res.status(200).json(order);
    }
    catch (error) {
        console.log(error);
        return res.status(400).json({ message: 'Failed prepare listing' });
    }
});
// Cancel listing
router.post('/cancelListing/skin', async (req, res) => {
    try {
        // Get the address of the seller
        let offererAddress = req.body.offererAddress ?? null;
        if (!offererAddress) {
            throw Error("Missng offererAddress");
        }
        // Get the listing id
        let listingId = req.body.listingId ?? null;
        if (!listingId) {
            throw Error("Missng listingId");
        }
        // Type of cancel
        let type = req.body.type ?? null;
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
        }
        else if (type == 'soft') {
            const { signableAction } = await client.prepareOrderCancellations([listingId]);
            return res.status(200).json({
                toSign: JSON.stringify(signableAction.message)
            });
        }
    }
    catch (error) {
        console.log(error);
        return res.status(400).json({ message: 'Failed prepare listing' });
    }
});
// Confirm cancel listing for soft cancel
router.post('/confirmCancelListing/skin', async (req, res) => {
    try {
        // Get the address of the seller
        let offererAddress = req.body.offererAddress ?? null;
        if (!offererAddress) {
            throw Error("Missng offererAddress");
        }
        // Get the listing id
        let listingId = req.body.listingId ?? null;
        if (!listingId) {
            throw Error("Missng listingId");
        }
        // Signature
        let signature = req.body.signature ?? null;
        if (!signature) {
            throw Error("Missing signature");
        }
        const response = await client.cancelOrders([listingId], offererAddress, signature);
        console.log(`response: ${response}`);
        return res.status(200).json(response);
    }
    catch (error) {
        console.log(error);
        return res.status(400).json({ message: 'Failed prepare listing' });
    }
});
// Fill order
router.post('/fillOrder/skin', async (req, res) => {
    try {
        // Get the address of the seller
        let fulfillerAddress = req.body.fulfillerAddress ?? null;
        if (!fulfillerAddress) {
            throw Error("Missng fulfillerAddress");
        }
        // Get the listing id
        let listingId = req.body.listingId ?? null;
        if (!listingId) {
            throw Error("Missng listingId");
        }
        // Get fees
        let fees = req.body.fees ?? null;
        if (!fees) {
            throw Error("Missng fees");
        }
        const feesValue = JSON.parse(fees);
        const { actions, expiration, order } = await client.fulfillOrder(listingId, fulfillerAddress, feesValue);
        console.log(`Fulfilling listing ${order}, transaction expiry ${expiration}`);
        const transactionsToSend = [];
        for (const action of actions) {
            if (action.type === sdk_1.orderbook.ActionType.TRANSACTION) {
                const builtTx = await action.buildTransaction();
                console.log(`Submitting ${action.purpose} transaction`);
                console.log(`Transaction to send ${builtTx.value}`);
                transactionsToSend.push(builtTx);
            }
        }
        console.log(`Number of transactions to send ${transactionsToSend.length}`);
        return res.status(200).json({ transactionsToSend: transactionsToSend });
    }
    catch (error) {
        console.log(error);
        return res.status(400).json({ message: 'Failed prepare listing' });
    }
});
app.use('/', router);
http_1.default.createServer(app).listen(6060, () => console.log('Listening on port 6060'));
