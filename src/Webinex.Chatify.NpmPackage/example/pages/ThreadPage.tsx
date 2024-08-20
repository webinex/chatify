import React from 'react';
import { useParams } from 'react-router-dom';
import { ThreadCustomizeValue, ThreadPanel } from '../../src';
import { Flippo } from '@webinex/flippo';
import { FLIPPO_AXIOS } from '../client';

const CUSTOMIZE: ThreadCustomizeValue = {
  flippo: new Flippo({ axios: FLIPPO_AXIOS }),
};

export function ThreadPage() {
  const { id } = useParams<'id'>();

  return (
    <div className="thread-page">
      <ThreadPanel threadId={id!} customize={CUSTOMIZE} />
    </div>
  );
}
