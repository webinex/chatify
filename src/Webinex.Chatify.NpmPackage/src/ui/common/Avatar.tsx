import { FC, ForwardedRef, forwardRef } from 'react';
import { Account } from '../../core';
import { Avatar as AntdAvatar } from 'antd';
import { customize } from '../customize';

export interface AvatarProps {
  account: Account;
}

const _Avatar: FC<AvatarProps> = forwardRef(
  (props: AvatarProps, forwardRef: ForwardedRef<HTMLSpanElement>) => {
    const { account } = props;

    return <AntdAvatar ref={forwardRef} src={account.avatar} />;
  },
);

_Avatar.displayName = 'Avatar';

export const Avatar = customize('Avatar', _Avatar);
