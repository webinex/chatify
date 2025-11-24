import { Chatify } from '@webinex/chatify';
import { AuditPanelCustomizeValue } from '../../src/audit';
import { Flippo } from '@webinex/flippo';
import { FLIPPO_AXIOS } from '../client';

const CUSTOMIZE: AuditPanelCustomizeValue = {
  flippo: new Flippo({ axios: FLIPPO_AXIOS }),
};

export function AuditPage() {
  return <Chatify.Audit customize={CUSTOMIZE} />;
}
