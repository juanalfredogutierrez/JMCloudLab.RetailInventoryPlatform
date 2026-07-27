import { InjectionToken } from '@angular/core';
import { RuntimeConfig } from './runtime-config.model';

export const RUNTIME_CONFIG =
  new InjectionToken<RuntimeConfig>('runtime-config');