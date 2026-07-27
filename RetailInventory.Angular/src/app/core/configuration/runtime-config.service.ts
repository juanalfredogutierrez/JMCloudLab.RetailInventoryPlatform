import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

import { RuntimeConfig } from './runtime-config.model';

@Injectable({
  providedIn: 'root'
})
export class RuntimeConfigService {

  private readonly http = inject(HttpClient);

  private config!: RuntimeConfig;

  async load(): Promise<void> {

    this.config = await firstValueFrom(
      this.http.get<RuntimeConfig>(
        '/assets/config/runtime-config.json'
      )
    );

  }

  get settings(): RuntimeConfig {
    return this.config;
  }

}