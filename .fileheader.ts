import { work, within, vc } from 'https://raw.githubusercontent.com/Liu233w/auto-file-header/master/mod.ts';

work((cfg) => {

  // cfg.versionControl = vc.git()

  cfg.include = [
    '*.js',
    '*.cs',
    '*.vue',
  ]
  cfg.excludeGlob = [
    'frontend/static/swagger/*',
    'crawler/crawlers/*',
    'backend/src/AcmStatisticsBackend.EntityFrameworkCore/Migrations/*',
    '**/.*',
    '**/*.config.js',
    '**/*.options.js',
    '**/node_modules/**',
    'backend/**/bin/**',
    'backend/**/obj/**',
    'e2e/**',
    '**/*test*/**',
    '**/__mocks__/**',
  ]

  cfg.default.format.trailingBlankLine = 1

  within(cfg.default.variables, (it) => {
    it.copyrightHolder = 'Shumin Liu and Contributors';
    it.projectStartYear = 2018;
  })

  cfg.languages['.vue'] = {
    format: {
      commentBegin: '<!-- ',
      commentPrefix: '  ',
      commentEnd: '-->',
    }
  }

  cfg.languages['.cs'] = {
    format: {
      commentBegin: '// ',
      commentPrefix: '// ',
      commentEnd: '',
      trailingBlankLine: 0,
    },
  }

  cfg.default.template = (v, f, p) =>
    `This file is part of the acm-statistics (https://github.com/Liu233w/acm-statistics)
Copyright (C) ${f.years(v)} ${v.copyrightHolder}

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.`
})
