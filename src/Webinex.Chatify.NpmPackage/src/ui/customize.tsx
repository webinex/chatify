import React, { FC, useContext } from 'react';

export type Customizable<TComponent extends React.ComponentType<any>> = TComponent & {
  Component: TComponent;
  key: string;
};

export const CustomizeContext = React.createContext<Record<string, any>>(null!);

export function useCustomizeContext<TValue>() {
  return useContext(CustomizeContext) as TValue;
}

function useCustom<TComponent extends React.ComponentType>(key: string): TComponent | undefined | null {
  const value = useContext(CustomizeContext);
  return value ? (value[key] as TComponent | null | undefined) : undefined;
}

export function useCustomize<TCustom extends Customizable<React.ComponentType<any>>>(custom: TCustom) {
  const Custom = useCustom<TCustom['Component']>(custom.key);

  if (Custom == null) {
    return null;
  }

  return Custom ?? custom.Component;
}

export function customize<TComponent extends React.ComponentType<any>>(
  key: string,
  Component: TComponent,
): TComponent extends React.ComponentType<infer X> ? Customizable<React.ComponentType<X>> : never {
  const Result: FC<any> = React.forwardRef<TComponent, any>((props: any, forwardRef) => {
    const Custom = useCustom(key);

    if (Custom === null) {
      return null;
    }

    if (Custom) {
      return <Custom {...props} ref={forwardRef} />;
    }

    return <Component {...props} ref={forwardRef} />;
  });

  Component.displayName = Component.displayName || key;
  Result.displayName = 'Customizable.' + Component.displayName;

  return Object.assign(Result, { key, Component }) as any;
}
