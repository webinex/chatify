import { FC } from 'react';
import { customize } from '../util';

const _Footer: FC = () => <div className="wxchtf-footer"></div>;
_Footer.displayName = 'Footer';

export const Footer = customize('Footer', _Footer);
