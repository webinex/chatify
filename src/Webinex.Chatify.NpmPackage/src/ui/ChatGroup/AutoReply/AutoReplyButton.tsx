import { Button } from 'antd';
import { customize } from '../../customize';
import { useChatGroupContext } from '../ChatGroupContext';
import { useLocalizer } from '../../localizer';
import { Icon } from '../../common/Icon';

export const AutoReplyButton = customize('AutoReplyButton', () => {
  const { toggleAutoReply } = useChatGroupContext();
  const localizer = useLocalizer();
  const titleText = localizer.autoReply.buttonTitle();

  return (
    <Button
      className="wxchtf-add-btn-settings"
      onClick={toggleAutoReply}
      icon={<Icon type="settings" />}
      title={titleText}
      aria-label={titleText}
    />
  );
});
