// @ts-check
// `@type` JSDoc annotations allow editor autocompletion and type checking
// (when paired with `@ts-check`).
// There are various equivalent ways to declare your Docusaurus config.
// See: https://docusaurus.io/docs/api/docusaurus-config

import {themes as prismThemes} from 'prism-react-renderer';

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'HARUBY',
  tagline: 'Dinosaurs are cool',
  favicon: 'favicon.png',

  // Set the production url of your site here
  url: 'https://www.haruby.com',
  // Set the /<baseUrl>/ pathname under which your site is served
  // For GitHub pages deployment, it is often '/<projectName>/'
  baseUrl: '/',

  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',

  // Even if you don't use internationalization, you can use this field to set
  // useful metadata like html lang. For example, if your site is Chinese, you
  // may want to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: './sidebars.js',
        },
        blog: {
          showReadingTime: true,
        },
        theme: {
          customCss: './src/css/custom.css',
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      navbar: {
        title: 'HARUBY',
        items: [
          {
            to: '/contact',
            label: 'Contact',
            position: 'right',
          },
          {
            to: '/about',
            label: 'About',
            position: 'right',
          },
        ],
      },
      footer: {
        style: 'dark',
        links: [
          {
            html: `
            <a class="footer__link-icon-item" href="https://blog.naver.com/haruby365" target="_blank" title="Visit Naver Blog">
              <img class="footer__link-icon" src="/images/socialIcons/naverBlog.svg" />
            </a>
            `,
          },
          {
            html: `
            <a class="footer__link-icon-item" href="http://pixiv.me/haruby" target="_blank" title="Visit Pixiv">
              <img class="footer__link-icon" src="/images/socialIcons/pixiv.svg" />
            </a>
            `,
          },
          {
            html: `
            <a class="footer__link-icon-item" href="https://www.artstation.com/haruby" target="_blank" title="Visit Artstation">
              <img class="footer__link-icon" src="/images/socialIcons/artstation.svg" />
            </a>
            `,
          },
          {
            html: `
            <a class="footer__link-icon-item" href="https://x.com/haruby365" target="_blank" title="Visit X">
              <img class="footer__link-icon" src="/images/socialIcons/x_gray.svg" />
            </a>
            `,
          },
        ],
        copyright: `© 2021-${new Date().getFullYear()} Jong-il Hong`,
      },
      prism: {
        theme: prismThemes.github,
        darkTheme: prismThemes.dracula,
      },
    }),
};

export default config;
