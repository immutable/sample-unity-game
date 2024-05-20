import { ethers } from "hardhat";
import { expect } from "chai";
import { RunnerToken, OperatorAllowlist__factory, RunnerToken__factory, ImmutableERC721, ImmutableERC721__factory } from "../typechain-types";

describe("RunnerToken", function () {
  let contract: RunnerToken;
  let skinContract: ImmutableERC721;

  beforeEach(async function () {
    // get owner (first account)
    const [owner] = await ethers.getSigners();

    // deploy OperatorAllowlist contract
    const OperatorAllowlist = await ethers.getContractFactory(
      "OperatorAllowlist"
    ) as OperatorAllowlist__factory;
    const operatorAllowlist = await OperatorAllowlist.deploy(owner.address);

    // deploy RunnerSkin contract
    const ImmutableERC721 = await ethers.getContractFactory("ImmutableERC721") as ImmutableERC721__factory;
    skinContract = await ImmutableERC721.deploy(
      owner.address, // owner
      "Immutable Runner Skin", // name
      "IMRS", // symbol
      "https://immutable.com/", // baseURI
      "https://immutable.com/", // contractURI
      await operatorAllowlist.getAddress(), // operator allowlist contract
      owner.address, // royalty recipient
      2000 // fee numerator
    );
    await skinContract.waitForDeployment();

    // deploy RunnerToken contract
    const RunnerToken = await ethers.getContractFactory("RunnerToken") as RunnerToken__factory;
    contract = await RunnerToken.deploy(await skinContract.getAddress());
    await contract.waitForDeployment();

    // grant owner the minter role
    await contract.grantMinterRole(owner.address);
    // grant the token contract to mint skins
    await skinContract.grantMinterRole(await contract.getAddress());
  });

  it("Should be deployed with the correct arguments", async function () {
    expect(await contract.name()).to.equal("Immutable Runner Token");
    expect(await contract.symbol()).to.equal("IMR");
  });

  it("Account with minter role should be able to mint tokens", async function () {
    const [owner, recipient] = await ethers.getSigners();

    await contract.connect(owner).mint(recipient.address, 1);
    expect(await contract.balanceOf(recipient.address)).to.equal(1);

    await contract.connect(owner).mint(recipient.address, 2);
    expect(await contract.balanceOf(recipient.address)).to.equal(3);
  });

  it("Account without minter role should not be able to mint NFTs", async function () {
    const [_, acc1] = await ethers.getSigners();
    const minterRole = await contract.MINTER_ROLE();
    await expect(
      contract.connect(acc1).mint(acc1.address, 1)
    ).to.be.revertedWith(
      `AccessControl: account ${acc1.address.toLowerCase()} is missing role ${minterRole}`
    );
  });

  it("Owner of tokens should be able to burn tokens", async function () {
    const [owner, recipient] = await ethers.getSigners();

    await contract.connect(owner).mint(recipient.address, 2);
    expect(await contract.balanceOf(recipient.address)).to.equal(2);

    await contract.connect(recipient).burn(1);
    expect(await contract.balanceOf(recipient.address)).to.equal(1);
  });

  it("Owner of tokens should not be able to burn tokens if not enough to burn", async function () {
    const [owner, recipient] = await ethers.getSigners();

    await contract.connect(owner).mint(recipient.address, 1);
    expect(await contract.balanceOf(recipient.address)).to.equal(1);

    await contract.connect(recipient).burn(1);
    expect(await contract.balanceOf(recipient.address)).to.equal(0);

    await expect(
      contract.connect(recipient).burn(1)
    ).to.be.revertedWith(`ERC20: burn amount exceeds balance`);
  });

  it("Others should not be able to burn others tokens", async function () {
    const [owner, acc1, recipient] = await ethers.getSigners();

    await contract.connect(owner).mint(recipient.address, 2);
    expect(await contract.balanceOf(recipient.address)).to.equal(2);

    await expect(
      contract.connect(acc1).burnFrom(recipient.address, 1)
    ).to.be.revertedWith(`ERC20: insufficient allowance`);
  });

  it("Account with three tokens can craft skin", async function () {
    const [owner, recipient] = await ethers.getSigners();

    const threeTokens = 3n * (10n ** await contract.decimals());
    await contract.connect(owner).mint(recipient.address, threeTokens);
    expect(await contract.balanceOf(recipient.address)).to.equal(threeTokens);

    await contract.connect(recipient).craftSkin();
    expect(await contract.balanceOf(recipient.address)).to.equal(0);
    expect(await skinContract.balanceOf(recipient.address)).to.equal(1);
    expect(await skinContract.ownerOf(await skinContract.mintBatchByQuantityThreshold())).to.equal(recipient.address);
  });

  it("Account with not enough tokens should not be able to craft skin", async function () {
    const [owner, recipient] = await ethers.getSigners();

    const twoTokens = 2n * (10n ** await contract.decimals());
    await contract.connect(owner).mint(recipient.address, twoTokens);
    expect(await contract.balanceOf(recipient.address)).to.equal(twoTokens);

    await expect(contract.connect(recipient).craftSkin()).to.be.revertedWith(
      'ERC20: burn amount exceeds balance'
    );
  });

  it("Token contract should not be able to craft skin without skin contract minter role", async function () {
    const [owner, recipient] = await ethers.getSigners();
    await skinContract.revokeMinterRole(await contract.getAddress());

    const threeTokens = 3n * (10n ** await contract.decimals());
    await contract.connect(owner).mint(recipient.address, threeTokens);
    expect(await contract.balanceOf(recipient.address)).to.equal(threeTokens);

    await expect(contract.connect(recipient).craftSkin()).to.be.revertedWith(
      `AccessControl: account ${(await contract.getAddress()).toLowerCase()} is missing role ${await skinContract.MINTER_ROLE()}`
    );
    expect(await contract.balanceOf(recipient.address)).to.equal(threeTokens);
  });

  it("Only admin account can grant minter role", async function () {
    const [owner, recipient] = await ethers.getSigners();

    await contract.connect(owner).grantMinterRole(recipient.address);
    expect(await contract.hasRole(await contract.MINTER_ROLE(), recipient.address)).to.equal(true);
  });

  it("Non-admin accounts cannot grant minter role", async function () {
    const [_, acc1, recipient] = await ethers.getSigners();
    const adminRole = await contract.DEFAULT_ADMIN_ROLE();

    await expect(
      contract.connect(acc1).grantMinterRole(recipient.address)
    ).to.be.revertedWith(
      `AccessControl: account ${acc1.address.toLowerCase()} is missing role ${adminRole}`
    );
  });
});