import { defineConfig } from 'cypress';

export default defineConfig({
  component: {
    devServer: {
      framework: 'angular',
      bundler: 'webpack',
    },
    specPattern: '**/*.spec.ts',
    chromeWebSecurity: false,
    viewportHeight: 1080,
    viewportWidth: 1920,
  },

  e2e: {
    baseUrl: 'https://localhost:4200',
    setupNodeEvents(on, config) {},
  },
});
