import express from "express";
import controller from "../controllers/posts";
import { mintFox, mintFoxToken } from "../controllers/fox";
import {
  inGameTradingMintFox,
  inGameTradingMintToken,
  inGameTradingMintSkin,
  balance,
  packs,
  packCheckApprovalRequired,
} from "../controllers/ingametrading";
const router = express.Router();

router.post("/mint/token", controller.mintToken);
router.post("/mint/character", controller.mintCharacter);
router.post("/mint/skin", controller.mintSkin);
router.get("/wallet", controller.wallet);
router.post("/zkmint/token", controller.zkMintToken);
router.post("/zkmint/character", controller.zkMintCharacter);
router.post("/zkmint/skin", controller.zkMintSkin);
router.post("/zk/token/craftskin/encodeddata", controller.zkTokenCraftSkinData);
router.post("/zk/skin/craftskin/encodeddata", controller.zkSkinCraftSkinData);
router.post("/fox/mint/token", mintFoxToken);
router.post("/fox/mint/fox", mintFox);

router.post("/ingametrading/mint/token", inGameTradingMintToken);
router.post("/ingametrading/mint/fox", inGameTradingMintFox);
router.post("/ingametrading/mint/skin", inGameTradingMintSkin);
router.get("/ingametrading/balance", balance);
router.get("/ingametrading/packs", packs);
router.post("/ingametrading/pack/checkApprovalRequired", packCheckApprovalRequired);

export = router;
