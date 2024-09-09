import { Wallet } from "@ethersproject/wallet";
import { Request, Response } from "express";
import env from "../config/client";
import { Contract, providers } from "ethers";

const gasOverrides = {
  // Use parameter to set tip for EIP1559 transaction (gas fee)
  maxPriorityFeePerGas: 10e9, // 10 Gwei. This must exceed minimum gas fee expectation from the chain
  maxFeePerGas: 15e9, // 15 Gwei
};

const zkEvmProvider = new providers.JsonRpcProvider(
  "https://rpc.testnet.immutable.com",
);

export const mintFoxToken = async (req: Request, res: Response) => {
  try {
    if (env.foxTokenAddress && env.privateKey) {
      // Get the address to mint the token to
      let to: string = req.body?.to ?? null;
      // Get the quantity to mint if specified, default is one
      let quantity = BigInt(req.body?.quantity ?? "1");
      // Connect to wallet with minter role
      const signer = new Wallet(env.privateKey).connect(zkEvmProvider);

      // Specify the function to call
      const abi = ["function mint(address to, uint256 quantity)"];
      // Connect contract to the signer
      const contract = new Contract(env.foxTokenAddress, abi, signer);

      // Mints the number of tokens specified
      const tx = await contract.mint(to, quantity, gasOverrides);
      await tx.wait();

      return res.status(200).json({});
    } else {
      return res.status(500).json({});
    }
  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: "Failed to mint token: " + error?.toString() });
  }
};

export const mintFox = async (req: Request, res: Response) => {
  try {
    if (env.foxContractAddress && env.privateKey) {
      // Get the address to mint the fox to
      let to: string = req.body.to ?? null;
      // Get the quantity to mint if specified, default is one
      let quantity = parseInt(req.body.quantity ?? "1");

      // Connect to wallet with minter role
      const signer = new Wallet(env.privateKey).connect(zkEvmProvider);

      // Specify the function to call
      const abi = ["function mintByQuantity(address to, uint256 quantity)"];
      // Connect contract to the signer
      const contract = new Contract(env.foxContractAddress, abi, signer);

      // Mints the number of tokens specified
      const tx = await contract.mintByQuantity(to, quantity, gasOverrides);
      await tx.wait();

      return res.status(200).json({});
    } else {
      return res.status(500).json({});
    }
  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: "Failed to mint skin: " + error?.toString() });
  }
};
