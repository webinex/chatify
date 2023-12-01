import { ChatHeader } from './ChatHeader';
import { ChatBody } from './ChatBody';
import { InputForm } from './InputForm';
import { useChat } from '../core';
import { useMe } from '../ChatifyContext';

export interface ChatProps {
  id: string;
}

export function Chat(props: ChatProps) {
  const { id } = props;
  const me = useMe();
  const { data: chat } = useChat({ chatId: id });
  const isMember = chat?.members.map((x) => x.id).includes(me) === true;

  return (
    <>
      <div className="wxchtf-chat">
        <ChatHeader id={id} />
        <ChatBody id={id} />
      </div>
      {isMember && <InputForm />}
    </>
  );
}
