import { Button } from 'antd';
import { customize } from '../../customize';
import { useChatGroupContext } from '../ChatGroupContext';
import { useLocalizer } from '../../localizer';
import { Icon } from '../../common/Icon';

export const AutoReplyButton = customize('AutoReplyButton', () => {
  const { toggleAutoReply } = useChatGroupContext();
  const localizer = useLocalizer();

  return (
    <Button
      className="wxchtf-add-btn-settings"
      onClick={toggleAutoReply}
      icon={<Icon type="settings" />}
      aria-label={String(localizer.autoReply.buttonTitle())}
    />
  );
});
