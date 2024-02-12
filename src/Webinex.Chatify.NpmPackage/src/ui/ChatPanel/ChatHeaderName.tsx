import { Input, Tooltip } from 'antd';
import { useSelect, useUpdateChatNameMutation } from '../../core';
import { ChatName } from '../ChatName';
import { customize } from '../customize';
import { useEffect, useState } from 'react';
import isHotkey from 'is-hotkey';

export interface ChatHeaderNameProps {
  id: string;
}

const isEscape = isHotkey('escape');
const isEnter = isHotkey('enter');

function useUpdateChatName(id: string, value: string) {
  const [updateChatName] = useUpdateChatNameMutation();
  return () => updateChatName({ id, name: value });
}

export const ChatHeaderName = customize('ChatHeaderName', (props: ChatHeaderNameProps) => {
  const { id } = props;
  const chat = useSelect((x) => x.query.chatList.data!.find((x) => x.id === id)!, [id]);
  const name = chat.name;
  const [editNameValue, setEditNameValue] = useState('');
  const [edit, setEdit] = useState(false);
  const updateChatName = useUpdateChatName(id, editNameValue);

  useEffect(() => {
    if (edit) {
      setEditNameValue(name);
    }
  }, [edit, name]);

  if (!chat.active) {
    return <span className="wxchtf-chat-name">{name}</span>;
  }

  if (edit) {
    return (
      <Input
        value={editNameValue}
        onChange={(e) => setEditNameValue(e.target.value)}
        onBlur={() => setEdit(false)}
        onKeyDown={(e) => {
          if (isEscape(e)) {
            setEdit(false);
          } else if (isEnter(e)) {
            setEdit(false);

            if (editNameValue !== name) {
              updateChatName();
            }
          }
        }}
        autoFocus
      />
    );
  }

  return (
    <Tooltip title="Click to edit name...">
      <span className="wxchtf-chat-name --editable" onClick={() => setEdit(true)}>
        <ChatName name={name} />
      </span>
    </Tooltip>
  );
});
