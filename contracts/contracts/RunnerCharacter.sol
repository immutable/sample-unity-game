// Copyright (c) Immutable Australia Pty Ltd 2018 - 2024
// SPDX-License-Identifier: MIT
pragma solidity 0.8.19;

import "@imtbl/contracts/contracts/token/erc721/preset/ImmutableERC721.sol";

// The Runner character you use to play the game
contract RunnerCharacter is ImmutableERC721 {
    uint256 private _currentTokenId = 0;

    constructor(
        address owner,
        string memory baseURI,
        string memory contractURI,
        address operatorAllowlist,
        address receiver,
        uint96 feeNumerator
    )
        ImmutableERC721(
            owner,
            "Immutable Runner Character",
            "IMRC",
            baseURI,
            contractURI,
            operatorAllowlist,
            receiver,
            feeNumerator
        )
    {}

    // Mints the next token
    function mintNextToken(address to) external onlyRole(MINTER_ROLE) returns (uint256) {
        uint256 tokenId = ++_currentTokenId;
        _mintByID(to, tokenId);
        return tokenId;
    }

    // Mints number of tokens specified
    function mintNextTokenByQuantity(address to, uint256 quantity) external onlyRole(MINTER_ROLE) {
        for (uint256 i = 0; i < quantity; i++) {
            _mintByID(to, ++_currentTokenId);
        }
    }
}