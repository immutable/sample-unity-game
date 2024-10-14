import express, {
  Express,
  Router,
  Request,
  Response,
} from 'express';
import cors from 'cors';
import http from 'http';
import { Wallet, Contract } from 'ethers';
import { JsonRpcProvider } from '@ethersproject/providers';
import morgan from 'morgan';
import dotenv from 'dotenv';

dotenv.config();

const app: Express = express();
app.use(morgan('dev')); // Logging
app.use(express.urlencoded({ extended: false })); // Parse request
app.use(express.json()); // Handle JSON
app.use(cors()); // Enable CORS
const router: Router = express.Router();

const zkEvmProvider = new JsonRpcProvider('https://rpc.testnet.immutable.com');

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
router.post('/mint/fox', async (req: Request, res: Response) => {
  console.log(req.body);
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

      res.writeHead(200);
      res.end(JSON.stringify({ message: "Minted foxes" }));
    } else {
      res.writeHead(400);
      res.end(JSON.stringify({ message: "Failed to mint" }));
    }
  } catch (error) {
    console.log(error);
    res.writeHead(500);
    res.end(JSON.stringify({ message: error }));
  }
});

// Mint Immutable Runner Token
router.post('/mint/token', async (req: Request, res: Response) => {
  console.log(req.body);
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

      res.writeHead(200);
      res.end(JSON.stringify({ message: "Minted ERC20 tokens" }));
    } else {
      res.writeHead(400);
      res.end(JSON.stringify({ message: "Failed to mint ERC20 tokens" }));
    }
  } catch (error) {
    console.log(error);
    res.writeHead(500);
    res.end(JSON.stringify({ message: error }));
  }
});

app.use('/', router);

http.createServer(app).listen(
  3000,
  () => console.log('Listening on port 3000'),
);
