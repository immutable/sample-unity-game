"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const cors_1 = __importDefault(require("cors"));
const http_1 = __importDefault(require("http"));
const ethers_1 = require("ethers");
const providers_1 = require("@ethersproject/providers");
const morgan_1 = __importDefault(require("morgan"));
const dotenv_1 = __importDefault(require("dotenv"));
dotenv_1.default.config();
const app = (0, express_1.default)();
app.use((0, morgan_1.default)('dev')); // Logging
app.use(express_1.default.urlencoded({ extended: false })); // Parse request
app.use(express_1.default.json()); // Handle JSON
app.use((0, cors_1.default)()); // Enable CORS
const router = express_1.default.Router();
const zkEvmProvider = new providers_1.JsonRpcProvider('https://rpc.testnet.immutable.com');
// Contract addresses
const foxContractAddress = process.env.FOX_CONTRACT_ADDRESS;
const tokenContractAddress = process.env.TOKEN_CONTRACT_ADDRESS;
// Private key of wallet with minter role
const privateKey = process.env.PRIVATE_KEY;
const gasOverrides = {
    // Use parameter to set tip for EIP1559 transaction (gas fee)
    maxPriorityFeePerGas: 10e9, // 10 Gwei. This must exceed minimum gas fee expectation from the chain
    maxFeePerGas: 15e9, // 15 Gwei
};
// Mint Immutable Runner Fox
router.post('/mint/fox', async (req, res) => {
    console.log(req.body);
    try {
        if (foxContractAddress && privateKey) {
            // Get the address to mint the fox to
            let to = req.body.to ?? null;
            // Get the quantity to mint if specified, default is one
            let quantity = parseInt(req.body.quantity ?? '1');
            // Connect to wallet with minter role
            const signer = new ethers_1.Wallet(privateKey).connect(zkEvmProvider);
            // Specify the function to call
            const abi = ["function mintNFT(address to)"];
            // Connect contract to the signer
            const contract = new ethers_1.Contract(foxContractAddress, abi, signer);
            // Mints 1 fox NFT
            const tx = await contract.mintNFT(to, gasOverrides);
            await tx.wait();
            res.writeHead(200);
            res.end(JSON.stringify({ message: "Minted foxes" }));
        }
        else {
            res.writeHead(400);
            res.end(JSON.stringify({ message: "Failed to mint" }));
        }
    }
    catch (error) {
        console.log(error);
        res.writeHead(500);
        res.end(JSON.stringify({ message: error }));
    }
});
// Mint Immutable Runner Token
router.post('/mint/token', async (req, res) => {
    console.log(req.body);
    try {
        if (tokenContractAddress && privateKey) {
            // Get the address to mint the token to
            let to = req.body.to ?? null;
            // Get the quantity to mint if specified, default is one
            let quantity = BigInt(req.body.quantity ?? '1');
            // Connect to wallet with minter role
            const signer = new ethers_1.Wallet(privateKey).connect(zkEvmProvider);
            // Specify the function to call
            const abi = ["function mintCoins(address to, uint256 quantity)"];
            // Connect contract to the signer
            const contract = new ethers_1.Contract(tokenContractAddress, abi, signer);
            // Mints the number of tokens specified
            const tx = await contract.mintCoins(to, quantity, gasOverrides);
            await tx.wait();
            res.writeHead(200);
            res.end(JSON.stringify({ message: "Minted ERC20 tokens" }));
        }
        else {
            res.writeHead(400);
            res.end(JSON.stringify({ message: "Failed to mint ERC20 tokens" }));
        }
    }
    catch (error) {
        console.log(error);
        res.writeHead(500);
        res.end(JSON.stringify({ message: error }));
    }
});
app.use('/', router);
http_1.default.createServer(app).listen(3000, () => console.log('Listening on port 3000'));
