import type { Component } from "vue";

export interface Action {
  name: string;
  invoke: CallableFunction;
  component: Component;
  order: number;
  icon?: string;
  display?: string;
  divider?: boolean;
  active?: CallableFunction;
  params?: Record<string, any>;
}
