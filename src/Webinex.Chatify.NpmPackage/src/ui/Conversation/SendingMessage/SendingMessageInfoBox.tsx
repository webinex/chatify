import { customize } from '../../customize';
import { useLocalizer } from '../../localizer';
import type { SendingMessageBoxProps } from './SendingMessageBox';

export const SendingMessageInfoBox = customize('SendingMessageInfoBox', (_: SendingMessageBoxProps) => {
  const localizer = useLocalizer();

  return (
    <div className="wxchtf-sending-message-info-box">
      <div className="wxchtf-sending-message-sending-box">{localizer.message.sending()}</div>
    </div>
  );
});
