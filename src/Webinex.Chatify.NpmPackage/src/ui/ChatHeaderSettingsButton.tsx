import { useDispatch } from '../core';
import { Button } from 'antd';
import { SettingOutlined } from '@ant-design/icons';
import { FC, useCallback } from 'react';
import { customize } from '../util';

export interface ChatSettingsButtonProps {
  id: string;
}

const _ChatSettingsButton: FC<ChatSettingsButtonProps> = (props) => {
  const { id } = props;
  const dispatch = useDispatch();
  const onSettingsClick = useCallback(
    () => dispatch({ type: 'chat_settings_open', data: { chatId: id } }),

    // eslint-disable-next-line react-hooks/exhaustive-deps
    [dispatch],
  );

  return <Button onClick={onSettingsClick} type="link" icon={<SettingOutlined />} />;
};

_ChatSettingsButton.displayName = 'ChatSettingsButton';

export const ChatSettingsButton = customize('ChatSettingsButton', _ChatSettingsButton);
