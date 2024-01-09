import { useEffect } from 'react';
import { useDispatch } from './reducer';
import { Account, ChatListItem, Message, ReadEvent } from './models';
import { ChatifyClient } from './client';

export function useSignalRMonitor(me: string, client: ChatifyClient) {
  const dispatch = useDispatch();

  useEffect(() => {
    let unsubscribes: Array<() => void> = [];

    client.connect().then(() => {
      unsubscribes.push(
        client.subscribe('chatify://new-message', ([message, requestId]: [Message, string]) => {
          dispatch({ type: 'message_received', data: { message, requestId } });
        }),
      );
      unsubscribes.push(
        client.subscribe('chatify://chat-created', ([chat, requestId]: [ChatListItem, string]) => {
          dispatch({ type: 'chat_created', data: { chat, requestId } });
        }),
      );
      unsubscribes.push(
        client.subscribe('chatify://read', ([events]: [ReadEvent[]]) => {
          dispatch({ type: 'read_received', data: { events } });
        }),
      );
      unsubscribes.push(
        client.subscribe(
          'chatify://member-added',
          ([chatId, chatName, account, message]: [string, string, Account, Message]) => {
            dispatch({ type: 'member_added', data: { chatId, chatName, account, me, message } });
          },
        ),
      );
      unsubscribes.push(
        client.subscribe(
          'chatify://member-removed',
          ([chatId, accountId, deleteHistory]: [string, string, boolean]) => {
            dispatch({ type: 'member_removed', data: { chatId, accountId, deleteHistory, me } });
          },
        ),
      );
    });

    return () => {
      unsubscribes.forEach((x) => x());
    };

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);
}
