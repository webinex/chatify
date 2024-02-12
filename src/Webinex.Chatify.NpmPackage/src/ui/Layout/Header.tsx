import { AddChatButton } from '../AddChatButton';
import { customize } from '../customize';

export const Header = customize('Header', () => {
  return (
    <div className="wxchtf-header">
      <AddChatButton />
    </div>
  );
});
