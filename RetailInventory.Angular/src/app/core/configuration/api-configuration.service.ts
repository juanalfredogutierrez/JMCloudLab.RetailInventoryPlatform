import { Injectable, inject } from '@angular/core';

import { RuntimeConfigService } from './runtime-config.service';

@Injectable({
  providedIn: 'root'
})
export class ApiConfigurationService {

  private readonly runtimeConfig = inject(RuntimeConfigService);

  get baseUrl(): string {
    return this.runtimeConfig.settings.apiUrl;
  }

}