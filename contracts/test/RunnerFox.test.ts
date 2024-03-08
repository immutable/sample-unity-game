import { ethers } from "hardhat";
import { expect } from "chai";
import { RunnerFox, OperatorAllowlist__factory, RunnerFox__factory } from "../typechain-types";

describe("RunnerFox", function () {
  let contract: RunnerFox;

  beforeEach(async function () {
    // get owner (first account)
    const [owner] = await ethers.getSigners();

    // deploy OperatorAllowlist contract
    const OperatorAllowlist = (await ethers.getContractFactory(
      "OperatorAllowlist"
    )) as OperatorAllowlist__factory;
    const operatorAllowlist = await OperatorAllowlist.deploy(owner.address);

    // deploy RunnerFox contract
    const RunnerFox = await ethers.getContractFactory("RunnerFox") as RunnerFox__factory;
    contract = await RunnerFox.deploy(
      owner.address, // owner
      "https://immutable.com/", // baseURI
      "https://immutable.com/", // contractURI
      operatorAllowlist.address, // operator allowlist contract
      owner.address, // royalty recipient
      ethers.BigNumber.from("2000") // fee numerator
    );
    await contract.deployed();

    // grant owner the minter role
    await contract.grantMinterRole(owner.address);
  });

  it("Should be deployed with the correct arguments", async function () {
    expect(await contract.name()).to.equal("Immutable Runner Fox");
    expect(await contract.symbol()).to.equal("IMRC");
    expect(await contract.baseURI()).to.equal("https://immutable.com/");
    expect(await contract.contractURI()).to.equal(
      "https://immutable.com/"
    );
  });

  it("Account with minter role should be able to mint next NFT", async function () {
    const [owner, recipient] = await ethers.getSigners();

    await contract.connect(owner).mintNextToken(recipient.address);
    expect(await contract.balanceOf(recipient.address)).to.equal(1);
    expect(await contract.totalSupply()).to.equal(1);
    expect(await contract.ownerOf(1)).to.equal(recipient.address); 

    await contract.connect(owner).mintNextToken(recipient.address);
    expect(await contract.balanceOf(recipient.address)).to.equal(2);
    expect(await contract.ownerOf(2)).to.equal(recipient.address);
  });

  it("Account with minter role should be able to mint next NFTs in batch", async function () {
    const [owner, recipient] = await ethers.getSigners();

    await contract.connect(owner).mintNextTokenByQuantity(recipient.address, 3);
    expect(await contract.balanceOf(recipient.address)).to.equal(3);
    expect(await contract.ownerOf(1)).to.equal(recipient.address);
    expect(await contract.ownerOf(2)).to.equal(recipient.address);
    expect(await contract.ownerOf(3)).to.equal(recipient.address);
  });

  it("Account without minter role should not be able to mint NFTs", async function () {
    const [_, acc1] = await ethers.getSigners();
    const minterRole = await contract.MINTER_ROLE();
    await expect(
      contract.connect(acc1).mintNextTokenByQuantity(acc1.address, 1)
    ).to.be.revertedWith(
      `AccessControl: account ${acc1.address.toLowerCase()} is missing role ${minterRole}`
    );
  });

  it("Account which owns the NFT, should be able to burn the NFT", async function () {
    const [owner, recipient] = await ethers.getSigners();

    await contract.connect(owner).mintNextTokenByQuantity(recipient.address, 1);
    expect(await contract.balanceOf(recipient.address)).to.equal(1);
    expect(await contract.ownerOf(1)).to.equal(recipient.address);

    await contract.connect(recipient).burn(1);
    expect(await contract.balanceOf(recipient.address)).to.equal(0);
  });

  it("Account which doesn't own the NFT, should not be able to burn the NFT", async function () {
    const [owner, acc1, recipient] = await ethers.getSigners();

    await contract.connect(owner).mintNextTokenByQuantity(recipient.address, 1);
    expect(await contract.balanceOf(recipient.address)).to.equal(1);
    expect(await contract.ownerOf(1)).to.equal(recipient.address);

    await expect(contract.connect(acc1).burn(1)).to.be.rejectedWith('IImmutableERC721NotOwnerOrOperator(1)');
  });
});