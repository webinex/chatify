import { FC } from 'react';
import { Account } from '../core';
import { Avatar as AntdAvatar } from 'antd';
import { customize } from './customize';

export interface AvatarProps {
  account: Account;
}

const _Avatar: FC<AvatarProps> = (props: AvatarProps) => {
  const { account } = props;

  return <AntdAvatar src={account.avatar} />;
};

_Avatar.displayName = 'Avatar';

export const Avatar = customize('Avatar', _Avatar);
