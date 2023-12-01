import { File } from '../core';
import { MessageMd } from './MessageMd';

export interface SendingMessageProps {
  text: string;
  files: File[];
}

export function SendingMessage(props: SendingMessageProps) {
  const { text, files } = props;
  return (
    <MessageMd text={text} files={files} my={true} sending={true} timestamp={new Date().toUTCString()} />
  );
}
