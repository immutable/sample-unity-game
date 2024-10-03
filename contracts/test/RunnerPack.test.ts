import { ethers } from 'hardhat';
import { expect } from 'chai';
// eslint-disable-next-line import/extensions
import { RunnerToken, OperatorAllowlist__factory, RunnerToken__factory, ImmutableERC721, ImmutableERC721__factory, RunnerPack, RunnerPack__factory } from '../typechain-types';
import { token } from '../typechain-types/@openzeppelin/contracts';

describe('RunnerShield', function () {
  let tokenContract: RunnerToken;
  let skinContract: ImmutableERC721;
  let contract: RunnerPack;

  beforeEach(async function () {
    // get owner (first account)
    const [owner] = await ethers.getSigners();

    // deploy OperatorAllowlist contract
    const OperatorAllowlist = await ethers.getContractFactory(
      'OperatorAllowlist'
    ) as unknown as OperatorAllowlist__factory;
    const operatorAllowlist = await OperatorAllowlist.deploy(owner.address);
    const operatorAllowlistAddress = await operatorAllowlist.getAddress();

    // deploy RunnerSkin contract
    const ImmutableERC721Contract = await ethers.getContractFactory('ImmutableERC721') as unknown as ImmutableERC721__factory;
    skinContract = await ImmutableERC721Contract.deploy(
      owner.address, // owner
      'Immutable Runner Skin', // name
      'IMRS', // symbol
      'https://immutable.com/', // baseURI
      'https://immutable.com/', // contractURI
      operatorAllowlistAddress, // operator allowlist contract
      owner.address, // royalty recipient
      2000, // fee numerator
    );
    await skinContract.waitForDeployment();

    // deploy RunnerToken contract
    const RunnerTokenContract = await ethers.getContractFactory('RunnerToken') as unknown as RunnerToken__factory;
    tokenContract = await RunnerTokenContract.deploy(await skinContract.getAddress());
    await tokenContract.waitForDeployment();

    // grant owner the minter role
    await tokenContract.grantMinterRole(owner.address);
    // grant the token contract to mint skins
    await skinContract.grantMinterRole(await tokenContract.getAddress());

    // deploy RunnerShield contract
    const RunnerPackContract = await ethers.getContractFactory('RunnerPack') as unknown as RunnerPack__factory;
    contract = await RunnerPackContract.deploy(
      await tokenContract.getAddress(),
      operatorAllowlistAddress);
    await contract.waitForDeployment();
  });

  it('Account with minter role should be able to mint tokens', async function () {
    const [owner, recipient] = await ethers.getSigners();

    const shieldId = await contract.GALACTIC_SHIELD();
    const clearId = await contract.CLEAR_SKIES();

    await contract.connect(owner).safeMint(recipient.address, shieldId.toString(), 5, "0x");
    expect(await contract.balanceOf(recipient.address, shieldId)).to.equal(5);

    await contract.connect(owner).safeMint(recipient.address, shieldId.toString(), 5, "0x");
    expect(await contract.balanceOf(recipient.address, shieldId)).to.equal(10);

    await contract.connect(owner).safeMint(recipient.address, clearId.toString(), 8, "0x");
    expect(await contract.balanceOf(recipient.address, clearId)).to.equal(8);
  });

  it('Account without minter role should not be able to mint NFTs', async function () {
    // eslint-disable-next-line @typescript-eslint/naming-convention, @typescript-eslint/no-unused-vars
    const [_, acc1] = await ethers.getSigners();
    const minterRole = await contract.MINTER_ROLE();

    const shieldId = await contract.GALACTIC_SHIELD();

    await expect(
      contract.connect(acc1).safeMint(acc1.address, shieldId.toString(), 5, "0x")
    ).to.be.revertedWith(
      `AccessControl: account ${acc1.address.toLowerCase()} is missing role ${minterRole}`,
    );
  });

  it('Owner of tokens should be able to burn tokens', async function () {
    const [owner, recipient] = await ethers.getSigners();

    const shieldId = await contract.GALACTIC_SHIELD();

    await contract.connect(owner).safeMint(recipient.address, shieldId.toString(), 2, "0x");
    expect(await contract.balanceOf(recipient.address, shieldId)).to.equal(2);

    await contract.connect(recipient).burn(recipient.address, shieldId, 1);
    expect(await contract.balanceOf(recipient.address, shieldId)).to.equal(1);
  });

  it('Owner of tokens should not be able to burn tokens if not enough to burn', async function () {
    const [owner, recipient] = await ethers.getSigners();

    const shieldId = await contract.GALACTIC_SHIELD();

    await contract.connect(owner).safeMint(recipient.address, shieldId.toString(), 1, "0x");
    expect(await contract.balanceOf(recipient.address, shieldId)).to.equal(1);

    await contract.connect(recipient).burn(recipient.address, shieldId, 1);
    expect(await contract.balanceOf(recipient.address, shieldId)).to.equal(0);

    await expect(
      contract.connect(recipient).burn(recipient.address, shieldId, 1)
    ).to.be.revertedWith('ERC1155: burn amount exceeds totalSupply');
  });

  it('Others should not be able to burn others tokens', async function () {
    const [owner, acc1, recipient] = await ethers.getSigners();

    const shieldId = await contract.GALACTIC_SHIELD();

    await contract.connect(owner).safeMint(recipient.address, shieldId.toString(), 2, "0x");
    expect(await contract.balanceOf(recipient.address, shieldId)).to.equal(2);

    await expect(
      contract.connect(acc1).burn(recipient.address, shieldId, 1)
    ).to.be.revertedWith('ERC1155: caller is not token owner or approved');
  });

  it('Account with ten tokens can buy galactic shield pack', async function () {
    const [owner, recipient] = await ethers.getSigners();

    const shieldId = await contract.GALACTIC_SHIELD();

    // Mint ten tokens to recipient
    const tenTokens = 10n * (10n ** await tokenContract.decimals());
    await tokenContract.connect(owner).mint(recipient.address, tenTokens);
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(tenTokens);

    // Approve the RunnerPack contract to spend tokens on behalf of recipient
    await tokenContract.connect(recipient).approve(await contract.getAddress(), tenTokens);

    // Buy pack
    await contract.connect(recipient).buyGalacticShieldPack();
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(0);
    expect(await contract.balanceOf(recipient.address, shieldId)).to.equal(5);
  });

  it('Account with not enough tokens should not be able to buy galactic shield pack', async function () {
    const [owner, recipient] = await ethers.getSigners();

    // Mint ten tokens to recipient
    const fiveTokens = 5n * (10n ** await tokenContract.decimals());
    await tokenContract.connect(owner).mint(recipient.address, fiveTokens);
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(fiveTokens);

    // Approve the RunnerPack contract to spend tokens on behalf of recipient
    await tokenContract.connect(recipient).approve(await contract.getAddress(), fiveTokens);

    await expect(contract.connect(recipient).buyGalacticShieldPack()).to.be.revertedWith(
      'ERC20: insufficient allowance',
    );
  });

  it('Without approval account cannot buy galactic shield pack', async function () {
    const [owner, recipient] = await ethers.getSigners();

    const shieldId = await contract.GALACTIC_SHIELD();

    // Mint ten tokens to recipient
    const tenTokens = 10n * (10n ** await tokenContract.decimals());
    await tokenContract.connect(owner).mint(recipient.address, tenTokens);
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(tenTokens);

    await expect(contract.connect(recipient).buyGalacticShieldPack()).to.be.revertedWith(
      'ERC20: insufficient allowance',
    );
  });

  it('Account with eight tokens can buy clear skies pack', async function () {
    const [owner, recipient] = await ethers.getSigners();

    const clearSkiesId = await contract.CLEAR_SKIES();

    // Mint eight tokens to recipient
    const eightTokens = 8n * (10n ** await tokenContract.decimals());
    await tokenContract.connect(owner).mint(recipient.address, eightTokens);
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(eightTokens);

    // Approve the RunnerPack contract to spend tokens on behalf of recipient
    await tokenContract.connect(recipient).approve(await contract.getAddress(), eightTokens);

    // Buy pack
    await contract.connect(recipient).buyClearSkiesPack();
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(0);
    expect(await contract.balanceOf(recipient.address, clearSkiesId)).to.equal(5);
  });

  it('Account with not enough tokens should not be able to buy clear skies pack', async function () {
    const [owner, recipient] = await ethers.getSigners();

    // Mint seven tokens to recipient
    const sevenTokens = 7n * (10n ** await tokenContract.decimals());
    await tokenContract.connect(owner).mint(recipient.address, sevenTokens);
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(sevenTokens);

    // Approve the RunnerPack contract to spend tokens on behalf of recipient
    await tokenContract.connect(recipient).approve(await contract.getAddress(), sevenTokens);

    await expect(contract.connect(recipient).buyClearSkiesPack()).to.be.revertedWith(
      'ERC20: insufficient allowance',
    );
  });

  it('Without approval account cannot buy clear skies pack', async function () {
    const [owner, recipient] = await ethers.getSigners();

    const clearSkiesId = await contract.CLEAR_SKIES();

    // Mint eight tokens to recipient
    const eightTokens = 8n * (10n ** await tokenContract.decimals());
    await tokenContract.connect(owner).mint(recipient.address, eightTokens);
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(eightTokens);

    await expect(contract.connect(recipient).buyClearSkiesPack()).to.be.revertedWith(
      'ERC20: insufficient allowance',
    );
  });

  it('Account with nine tokens can buy navigators combo pack', async function () {
    const [owner, recipient] = await ethers.getSigners();

    const shieldId = await contract.GALACTIC_SHIELD();
    const clearSkiesId = await contract.CLEAR_SKIES();

    // Mint nine tokens to recipient
    const nineTokens = 9n * (10n ** await tokenContract.decimals());
    await tokenContract.connect(owner).mint(recipient.address, nineTokens);
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(nineTokens);

    // Approve the RunnerPack contract to spend tokens on behalf of recipient
    await tokenContract.connect(recipient).approve(await contract.getAddress(), nineTokens);

    // Buy pack
    await contract.connect(recipient).buyNavigatorsComboPack();
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(0);
    expect(await contract.balanceOf(recipient.address, shieldId)).to.equal(3);
    expect(await contract.balanceOf(recipient.address, clearSkiesId)).to.equal(3);
  });

  it('Account with not enough tokens should not be able to buy navigators combo pack', async function () {
    const [owner, recipient] = await ethers.getSigners();

    // Mint eight tokens to recipient
    const eightTokens = 8n * (10n ** await tokenContract.decimals());
    await tokenContract.connect(owner).mint(recipient.address, eightTokens);
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(eightTokens);

    // Approve the RunnerPack contract to spend tokens on behalf of recipient
    await tokenContract.connect(recipient).approve(await contract.getAddress(), eightTokens);

    await expect(contract.connect(recipient).buyNavigatorsComboPack()).to.be.revertedWith(
      'ERC20: insufficient allowance',
    );
  });

  it('Without approval account cannot buy navigators combo pack', async function () {
    const [owner, recipient] = await ethers.getSigners();

    const shieldId = await contract.GALACTIC_SHIELD();
    const clearSkiesId = await contract.CLEAR_SKIES();

    // Mint nine tokens to recipient
    const nineTokens = 9n * (10n ** await tokenContract.decimals());
    await tokenContract.connect(owner).mint(recipient.address, nineTokens);
    expect(await tokenContract.balanceOf(recipient.address)).to.equal(nineTokens);

    await expect(contract.connect(recipient).buyNavigatorsComboPack()).to.be.revertedWith(
      'ERC20: insufficient allowance',
    );
  });

});