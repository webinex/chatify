import { customize } from '../customize';
import { ChatList } from '../ChatList';

export const Aside = customize('Aside', () => {
  return (
    <div className="wxchtf-aside">
      <ChatList />
    </div>
  );
});
