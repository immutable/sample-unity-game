import { Request, Response } from "express";
import { providers, Wallet, Contract, utils, BigNumber } from "ethers";
import env from "../config/client";

const zkEvmProvider = new providers.JsonRpcProvider(
  `https://rpc.testnet.immutable.com`,
);

// Contract addresses
const foxContractAddress = env.inGameTradingFoxAddress;
const tokenContractAddress = env.inGameTradingTokenAddress;
const skinColourContractAddress = env.inGameTradingSkinColourAddress;
const packContractAddress = env.inGameTradingPackAddress;
// Private key of wallet with minter role
const privateKey = env.privateKey;

const gasOverrides = {
  // Use parameter to set tip for EIP1559 transaction (gas fee)
  maxPriorityFeePerGas: 10e9, // 10 Gwei. This must exceed minimum gas fee expectation from the chain
  maxFeePerGas: 15e9, // 15 Gwei
};

// Mint Immutable Runner Fox
export const inGameTradingMintFox = async (req: Request, res: Response) => {
  try {
    if (foxContractAddress && privateKey) {
      // Get the address to mint the fox to
      const to: string = req.body.to ?? null;
      // Get the quantity to mint if specified, default is one
      const quantity = parseInt(req.body.quantity ?? "1");

      // Connect to wallet with minter role
      const signer = new Wallet(privateKey).connect(zkEvmProvider);

      // Specify the function to call
      const abi = ["function mintByQuantity(address to, uint256 quantity)"];
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
    return res.status(400).json({ message: "Failed to mint to user" });
  }
};

// Mint Immutable Runner Token
export const inGameTradingMintToken = async (req: Request, res: Response) => {
  try {
    if (tokenContractAddress && privateKey) {
      // Get the address to mint the token to
      const to: string = req.body.to ?? null;
      // Get the quantity to mint if specified, default is one
      const quantity = BigInt(req.body.quantity ?? "1");

      // Connect to wallet with minter role
      const signer = new Wallet(privateKey).connect(zkEvmProvider);

      // Specify the function to call
      const abi = ["function mint(address to, uint256 quantity)"];
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
    return res.status(400).json({ message: "Failed to mint to user" });
  }
};

export const inGameTradingMintSkin = async (req: Request, res: Response) => {
  try {
    if (skinColourContractAddress && privateKey) {
      // Get the address to mint the token to
      const to: string = req.body.to ?? null;
      // Get the quantity to mint if specified, default is one
      const tokenId = BigInt(req.body.tokenId ?? "1");

      // Connect to wallet with minter role
      const signer = new Wallet(privateKey).connect(zkEvmProvider);

      // Specify the function to call
      const abi = ["function mint(address to, uint256 tokenId)"];
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
    return res.status(400).json({ message: "Failed to mint to user" });
  }
};

// In-game ERC20 balance
export const balance = async (req: Request, res: Response) => {
  try {
    if (tokenContractAddress) {
      // Get the address
      const address = req.query.address ?? null;

      // Call balanceOf
      const abi = [
        "function balanceOf(address account) view returns (uint256)",
      ];
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
    return res.status(400).json({ message: "Failed to mint to user" });
  }
};

// Packs
const galacticShieldId = 1;
const clearSkiesId = 2;
export const packs = async (req: Request, res: Response) => {
  try {
    if (packContractAddress) {
      const packs = [
        {
          name: "Galactic Shield Pack",
          description:
            "Gain immunity to obstacles with Galactic Shields. Move through anything in your way.",
          items: [
            {
              id: galacticShieldId,
              name: "Galactic Shield",
              amount: 5,
              image:
                "https://cyan-electric-peafowl-878.mypinata.cloud/ipfs/QmSA7X4Jxq2k8oTAricFrYrTrgXajLBLKvVoSfZoM6z4pF",
            },
          ],
          collection: packContractAddress,
          image:
            "https://cyan-electric-peafowl-878.mypinata.cloud/ipfs/QmSA7X4Jxq2k8oTAricFrYrTrgXajLBLKvVoSfZoM6z4pF",
          price: "10000000000000000000",
          function: "0x64f54bf2",
        },
        {
          name: "Clear Skies Pack",
          description:
            "Remove cosmic hazards like meteor showers and clear your path for a smooth run.",
          items: [
            {
              id: clearSkiesId,
              name: "Clear Skies",
              amount: 5,
              image:
                "https://cyan-electric-peafowl-878.mypinata.cloud/ipfs/QmQe7mvDqKiTj6kZqjWzHto64kY64pub9KbxRcYSx3gtHm",
            },
          ],
          collection: packContractAddress,
          image:
            "https://cyan-electric-peafowl-878.mypinata.cloud/ipfs/QmQe7mvDqKiTj6kZqjWzHto64kY64pub9KbxRcYSx3gtHm",
          price: "8000000000000000000",
          function: "0xd69c42be",
        },
        {
          name: "Navigator’s Combo Pack",
          description:
            "Get both Galactic Shields and Clear Skies to manage obstacles your way.",
          items: [
            {
              id: galacticShieldId,
              name: "Galactic Shield",
              amount: 3,
              image:
                "https://cyan-electric-peafowl-878.mypinata.cloud/ipfs/QmSA7X4Jxq2k8oTAricFrYrTrgXajLBLKvVoSfZoM6z4pF",
            },
            {
              id: clearSkiesId,
              name: "Clear Skies",
              amount: 3,
              image:
                "https://cyan-electric-peafowl-878.mypinata.cloud/ipfs/QmQe7mvDqKiTj6kZqjWzHto64kY64pub9KbxRcYSx3gtHm",
            },
          ],
          collection: packContractAddress,
          image:
            "https://cyan-electric-peafowl-878.mypinata.cloud/ipfs/QmfPRGUwQ8XisoJR42HwkTYmeUid7vDcPDrMRetwhvZY31",
          price: "9000000000000000000",
          function: "0x9c1af459",
        },
      ];

      return res.status(200).json({
        result: packs,
      });
    } else {
      return res.status(500).json({});
    }
  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: "Failed to mint to user" });
  }
};

// Approve pack contract to spend token
export const packCheckApprovalRequired = async (
  req: Request,
  res: Response,
) => {
  try {
    if (tokenContractAddress && packContractAddress && privateKey) {
      const address: string = req.body.address ?? null;
      const amount: string = req.body.amount ?? null;

      // Call allowance
      const abi = [
        "function allowance(address owner, address spender) view returns (uint256)",
        "function approve(address spender, uint256 amount)",
      ];

      const tokenContract = new Contract(
        tokenContractAddress,
        abi,
        zkEvmProvider,
      );
      const approvedAmount = await tokenContract.allowance(
        address,
        packContractAddress,
      );
      const approvedAmountDecimal = approvedAmount.toString();
      console.log(`approvedAmount: ${approvedAmount}`);

      if (BigNumber.from(approvedAmountDecimal).lt(BigNumber.from(amount))) {
        console.log("The approved amount is less than the requested amount.");
        const iface = new utils.Interface(abi);
        const encodedData = iface.encodeFunctionData("approve", [
          packContractAddress,
          amount,
        ]);
        console.log(`encodedData: ${encodedData}`);

        return res.status(200).json({
          to: tokenContractAddress,
          data: encodedData,
          amount: 0,
        });
      } else {
        console.log("The approved amount is sufficient.");
        return res.status(200).json({});
      }
    } else {
      return res.status(500).json({});
    }
  } catch (error) {
    console.log(error);
    return res.status(400).json({ message: "Failed to encode" });
  }
};
