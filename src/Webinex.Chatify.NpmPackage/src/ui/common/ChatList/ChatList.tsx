import { Menu, MenuProps } from 'antd';
import { useMemo } from 'react';
import { ChatListItemBox } from './ChatListItemBox';
import { customize } from '../../customize';
import { ChatListValue } from './ChatListValue';
import { useLocalizer } from '../../localizer';
import { messageSequencePart } from '../../../core';

export interface ChatListProps {
  className?: string;
  items: ChatListValue[];
  selected: string | null;
  onSelect?: (id: string) => void;

  /**
   * If true, do not show read status.
   */
  noRead?: boolean;

  /**
   * If set, shows a message at the bottom indicating that only the top N chats are shown.
   */
  top?: number;
}

function useOnSelect(props: ChatListProps) {
  const { onSelect } = props;

  return useMemo(
    () =>
      onSelect
        ? (info: Parameters<NonNullable<MenuProps['onSelect']>>[0]) => onSelect(info.key as string)
        : undefined,
    [onSelect],
  );
}

function useMenuItems(props: ChatListProps) {
  //28b2a1a6-c052-4761-ad2d-95ce36d59f06-000000001
  const { items, noRead, top } = props;
  const localizer = useLocalizer();

  return useMemo<MenuProps['items']>(() => {
    const result = items
      ?.slice()
      .sort(
        (a, b) => -messageSequencePart(a.lastMessage.id).localeCompare(messageSequencePart(b.lastMessage.id)),
      )
      .map((x) => ({ key: x.id, label: <ChatListItemBox chat={x} noRead={noRead} /> }));

    if (top != null) {
      return [
        ...result,
        {
          type: 'item',
          key: 'top',
          disabled: true,
          label: (
            <div style={{ textAlign: 'center', cursor: 'text', fontStyle: 'italic' }}>
              {localizer.chatList.topShown(top)}
            </div>
          ),
        },
      ];
    }

    return result;
  }, [items, noRead, top, localizer]);
}

export const ChatList = customize('ChatList', (props: ChatListProps) => {
  const { selected: selectedChatId } = props;
  const selected = useMemo(() => (selectedChatId ? [selectedChatId] : []), [selectedChatId]);
  const onSelect = useOnSelect(props);
  const items = useMenuItems(props);

  return (
    <>
      {items && (
        <Menu className="wxchtf-chat-list" selectedKeys={selected} onSelect={onSelect} items={items} />
      )}
    </>
  );
});
