import { ethers } from 'hardhat'; // eslint-disable-line import/no-extraneous-dependencies

async function main() {
  // Load the Immutable Runner Tokencontract and get the contract factory
  const contractFactory = await ethers.getContractFactory('RunnerToken1155');

  // Deploy the contract to the zkEVM network
  const ownerEthAddress = ""
  const collectionName = "Immutable Runner Fox 1155"
  const baseURI = "https://immutable-runner-json-server-0q8u.onrender.com/fox/"
  const contractURI = "https://crimson-specified-vicuna-718.mypinata.cloud/ipfs/QmY7zUHJ6ti3Vg1NPBEjpsNELaMrHvJLwyXQzRd1Pjj68H"
  const contract = await contractFactory.deploy(
    ownerEthAddress,
    collectionName,
    baseURI,
    contractURI, // contract / collection metadata URI
    "0x6b969FD89dE634d8DE3271EbE97734FEFfcd58eE", // operator allow list  https://docs.immutable.com/products/zkevm/contracts/erc721/#operator-allowlist-royalty-and-protocol-fee-enforcement
    ownerEthAddress, // royalty recipient
    5, // royalty fee percentage
    "0x9b74823f4bcbcb3e0f74cadc3a1d2552a18777ed" // Immutable Runner Skin contract address
  );

  console.log('Contract deployed to:', await contract.getAddress());
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });