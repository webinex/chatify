import React, { FC, PropsWithChildren, useContext } from 'react';

export type Customizable<TComponent extends React.ComponentType<any>> = TComponent & {
  Component: TComponent;
};

const CustomizeContext = React.createContext<CustomizeProviderValue>(null!);

export interface CustomizeProviderValue {
  [key: string]: React.ComponentType<any> | null;
}

export function CustomizeProvider(props: PropsWithChildren<{ value: CustomizeProviderValue }>) {
  const { children, value } = props;
  return <CustomizeContext.Provider value={value}>{children}</CustomizeContext.Provider>;
}

function useCustom<TComponent extends React.ComponentType>(key: string): TComponent | undefined | null {
  const value = useContext(CustomizeContext);
  return value[key] as TComponent | null | undefined;
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
