import { useDispatch } from '../../core';
import { Button } from 'antd';
import { useCallback } from 'react';
import { customize } from '../customize';
import { Icon } from '../Icon';

export interface ChatMembersButtonProps {
  id: string;
}

export const ChatMembersButton = customize('ChatMembersButton', (_: ChatMembersButtonProps) => {
  const dispatch = useDispatch();
  const onSettingsClick = useCallback(
    () => dispatch({ type: 'toggle_members_view', data: void 0 }),

    // eslint-disable-next-line react-hooks/exhaustive-deps
    [dispatch],
  );

  return <Button onClick={onSettingsClick} type="link" icon={<Icon type="team" />} />;
});
