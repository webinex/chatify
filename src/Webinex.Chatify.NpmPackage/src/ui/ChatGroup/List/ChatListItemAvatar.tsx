import { color } from '../../color';
import { Icon } from '../../common/Icon';
import { customize } from '../../customize';
import type { ChatListItemBoxProps } from './ChatListItemBox';

export const ChatListItemAvatar = customize('ChatListItemAvatar', (props: ChatListItemBoxProps) => {
  const { id } = props.chat;

  return (
    <div
      className="wxchtf-chat-avatar"
      style={
        {
          '--background-color': color(id, { lightness: 90 }),
          '--color': color(id, { saturation: 60, lightness: 65 }),
        } as any
      }
    >
      <Icon type="chat" />
    </div>
  );
});
