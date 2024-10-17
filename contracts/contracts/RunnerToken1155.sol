// Copyright Immutable Pty Ltd 2018 - 2023
// SPDX-License-Identifier: Apache 2.0
pragma solidity 0.8.19;

import "@imtbl/contracts/contracts/token/erc1155/preset/ImmutableERC1155.sol";
import "@imtbl/contracts/contracts/token/erc721/preset/ImmutableERC721.sol";


/**
 * @title ImmutableERC1155
 * @author @jasonzwli, Immutable
 */
contract RunnerToken1155 is ImmutableERC1155 {
    ///     =====   Constructor  =====
    uint256 public constant FOXTOKENID = 1;
    uint256 public constant COINTOKENID = 2;

    // A reference to the Immutable Runner Skin contract for the craftSkin function to use.
    // Note: Immutable Runner Skin contract simply extends ImmutableERC721, so we can set
    // the type to ImmutableERC721
    ImmutableERC721 private _skinContract;

    /**
     * @notice Grants `DEFAULT_ADMIN_ROLE` to the supplied `owner` address
     *
     * Sets the name and symbol for the collection
     * Sets the default admin to `owner`
     * Sets the `baseURI`
     * Sets the royalty receiver and amount (this can not be changed once set)
     * @param owner The address that will be granted the `DEFAULT_ADMIN_ROLE`
     * @param name_ The name of the collection
     * @param baseURI_ The base URI for the collection
     * @param contractURI_ The contract URI for the collection
     * @param _operatorAllowlist The address of the OAL
     * @param _receiver The address that will receive the royalty payments
     * @param _feeNumerator The percentage of the sale price that will be paid as a royalty
     */
    constructor(
        address owner,
        string memory name_,
        string memory baseURI_,
        string memory contractURI_,
        address _operatorAllowlist,
        address _receiver,
        uint96 _feeNumerator,
        address skinContractAddr
    ) ImmutableERC1155(owner, name_, baseURI_, contractURI_, _operatorAllowlist, _receiver, _feeNumerator) {
        // Grant the contract deployer the default admin role: it will be able
        // to grant and revoke any roles
        _setupRole(DEFAULT_ADMIN_ROLE, msg.sender);
        // Save the Immutable Runner Skin contract address
        _skinContract = ImmutableERC721(skinContractAddr);

        // Uncomment the line below to grant minter role to contract deployer
        _grantRole(MINTER_ROLE, msg.sender);
    }

    function mintNFT(address to) external onlyRole(MINTER_ROLE) {
        super._mint(to, FOXTOKENID, 1, "");
    }

    function mintCoins(address to, uint256 quantity) external onlyRole(MINTER_ROLE) {
        super._mint(to, COINTOKENID, quantity, "");
    }

    // Burns three tokens and crafts a skin to the caller
    function craftSkin() external {
        require(
            balanceOf(msg.sender, COINTOKENID) >= 3,
            "craftSkin: Caller does not have enough tokens"
        );

        // Burn caller's three tokens
        _burn(msg.sender, COINTOKENID, 3);

        // Mint one Immutable Runner Skin to the caller
        // Note: To mint a skin, the Immutable Runner Token contract must have the
        // Immutable Runner Skin contract minter role.
        _skinContract.mintByQuantity(msg.sender, 1);
    }
}
