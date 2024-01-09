import { FC } from 'react';
import { customize } from '../util';
import SendOutlined from '@ant-design/icons/SendOutlined';
import FileAddOutlined from '@ant-design/icons/FileAddOutlined';

export type IconType = 'attach' | 'send';

export interface IconProps {
  type: IconType;
}

export const ICONS: Record<IconType, React.ComponentType> = {
  attach: FileAddOutlined,
  send: SendOutlined,
};

export const Icon = customize<FC<IconProps>, string>('Icon', (props) => {
  const { type } = props;
  const IconComponent = ICONS[type];
  return IconComponent ? <IconComponent /> : null;
});
