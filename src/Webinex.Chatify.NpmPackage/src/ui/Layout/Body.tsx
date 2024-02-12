import { customize } from '../customize';
import { Aside } from './Aside';
import { Main } from './Main';

export const Body = customize('Body', () => (
  <div className="wxchtf-body">
    <Aside />
    <Main />
  </div>
));
