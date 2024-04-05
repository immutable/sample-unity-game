import express, {
  Express, 
  Router,
  Request,
  Response 
} from 'express';
import cors from 'cors';
import http from 'http';

const foxImageUrl = 'https://rose-ministerial-termite-701.mypinata.cloud/ipfs/Qmd3oT99HypRHaPfiY6JWokxADR5TzR1stgonFy1rMZAUy'
const skinImageUrl = 'https://rose-ministerial-termite-701.mypinata.cloud/ipfs/QmNZeG8wkW3mFw4PrqEj34NPA88impcvemYjhAkJAM4YcK';

const app: Express = express();
app.use(cors());

const router: Router = express.Router();

router.get(
  '/fox/:id',
  async (req: Request, res: Response) => {
    const json = {
      id: parseInt(req.params.id),
      name: `Fox #${req.params.id}`,
      image: foxImageUrl
    };
    
    res.writeHead(200);

    res.end(JSON.stringify(json));
  }
);

router.get(
  '/skin/:id',
  async (req: Request, res: Response) => {
    const json = {
      id: parseInt(req.params.id),
      name: `Skin #${req.params.id}`,
      image: skinImageUrl
    };
    
    res.writeHead(200);

    res.end(JSON.stringify(json));
  }
);

app.use('/', router);

http.createServer(app).listen(
  3000, 
  () => console.log(`Listening on port 3000`)
);