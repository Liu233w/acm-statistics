export const WORKER_STATUS = {
  WAITING: 'WAITING',
  WORKING: 'WORKING',
  DONE: 'DONE',
}

export const PROJECT_TITLE = 'NWPU-ACM 查询系统'

export const TIMEZONE_LIST = [
  { value: 'UTC', text: '(GMT) UTC' },
  { value: 'Morocco Standard Time', text: '(GMT) Casablanca' },
  { value: 'GMT Standard Time', text: '(GMT) Greenwich Mean Time : Dublin, Edinburgh, Lisbon, London' },
  { value: 'Greenwich Standard Time', text: '(GMT) Monrovia, Reykjavik' },
  { value: 'W. Europe Standard Time', text: '(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna' },
  { value: 'Central Europe Standard Time', text: '(GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague' },
  { value: 'Romance Standard Time', text: '(GMT+01:00) Brussels, Copenhagen, Madrid, Paris' },
  { value: 'Central European Standard Time', text: '(GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb' },
  { value: 'W. Central Africa Standard Time', text: '(GMT+01:00) West Central Africa' },
  { value: 'Jordan Standard Time', text: '(GMT+02:00) Amman' },
  { value: 'GTB Standard Time', text: '(GMT+02:00) Athens, Bucharest, Istanbul' },
  { value: 'Middle East Standard Time', text: '(GMT+02:00) Beirut' },
  { value: 'Egypt Standard Time', text: '(GMT+02:00) Cairo' },
  { value: 'South Africa Standard Time', text: '(GMT+02:00) Harare, Pretoria' },
  { value: 'FLE Standard Time', text: '(GMT+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius' },
  { value: 'Israel Standard Time', text: '(GMT+02:00) Jerusalem' },
  { value: 'E. Europe Standard Time', text: '(GMT+02:00) Minsk' },
  { value: 'Namibia Standard Time', text: '(GMT+02:00) Windhoek' },
  { value: 'Arabic Standard Time', text: '(GMT+03:00) Baghdad' },
  { value: 'Arab Standard Time', text: '(GMT+03:00) Kuwait, Riyadh' },
  { value: 'Russian Standard Time', text: '(GMT+03:00) Moscow, St. Petersburg, Volgograd' },
  { value: 'E. Africa Standard Time', text: '(GMT+03:00) Nairobi' },
  { value: 'Georgian Standard Time', text: '(GMT+03:00) Tbilisi' },
  { value: 'Iran Standard Time', text: '(GMT+03:30) Tehran' },
  { value: 'Arabian Standard Time', text: '(GMT+04:00) Abu Dhabi, Muscat' },
  { value: 'Azerbaijan Standard Time', text: '(GMT+04:00) Baku' },
  { value: 'Mauritius Standard Time', text: '(GMT+04:00) Port Louis' },
  { value: 'Caucasus Standard Time', text: '(GMT+04:00) Yerevan' },
  { value: 'Afghanistan Standard Time', text: '(GMT+04:30) Kabul' },
  { value: 'Ekaterinburg Standard Time', text: '(GMT+05:00) Ekaterinburg' },
  { value: 'Pakistan Standard Time', text: '(GMT+05:00) Islamabad, Karachi' },
  { value: 'West Asia Standard Time', text: '(GMT+05:00) Tashkent' },
  { value: 'India Standard Time', text: '(GMT+05:30) Chennai, Kolkata, Mumbai, New Delhi' },
  { value: 'Sri Lanka Standard Time', text: '(GMT+05:30) Sri Jayawardenepura' },
  { value: 'Nepal Standard Time', text: '(GMT+05:45) Kathmandu' },
  { value: 'N. Central Asia Standard Time', text: '(GMT+06:00) Almaty, Novosibirsk' },
  { value: 'Central Asia Standard Time', text: '(GMT+06:00) Astana, Dhaka' },
  { value: 'Myanmar Standard Time', text: '(GMT+06:30) Yangon (Rangoon)' },
  { value: 'SE Asia Standard Time', text: '(GMT+07:00) Bangkok, Hanoi, Jakarta' },
  { value: 'North Asia Standard Time', text: '(GMT+07:00) Krasnoyarsk' },
  { value: 'China Standard Time', text: '(GMT+08:00) Beijing, Chongqing, Hong Kong, Urumqi' },
  { value: 'North Asia East Standard Time', text: '(GMT+08:00) Irkutsk, Ulaan Bataar' },
  { value: 'Singapore Standard Time', text: '(GMT+08:00) Kuala Lumpur, Singapore' },
  { value: 'W. Australia Standard Time', text: '(GMT+08:00) Perth' },
  { value: 'Taipei Standard Time', text: '(GMT+08:00) Taipei' },
  { value: 'Tokyo Standard Time', text: '(GMT+09:00) Osaka, Sapporo, Tokyo' },
  { value: 'Korea Standard Time', text: '(GMT+09:00) Seoul' },
  { value: 'Yakutsk Standard Time', text: '(GMT+09:00) Yakutsk' },
  { value: 'Cen. Australia Standard Time', text: '(GMT+09:30) Adelaide' },
  { value: 'AUS Central Standard Time', text: '(GMT+09:30) Darwin' },
  { value: 'E. Australia Standard Time', text: '(GMT+10:00) Brisbane' },
  { value: 'AUS Eastern Standard Time', text: '(GMT+10:00) Canberra, Melbourne, Sydney' },
  { value: 'West Pacific Standard Time', text: '(GMT+10:00) Guam, Port Moresby' },
  { value: 'Tasmania Standard Time', text: '(GMT+10:00) Hobart' },
  { value: 'Vladivostok Standard Time', text: '(GMT+10:00) Vladivostok' },
  { value: 'Central Pacific Standard Time', text: '(GMT+11:00) Magadan, Solomon Is., New Caledonia' },
  { value: 'New Zealand Standard Time', text: '(GMT+12:00) Auckland, Wellington' },
  { value: 'Fiji Standard Time', text: '(GMT+12:00) Fiji, Kamchatka, Marshall Is.' },
  { value: 'Tonga Standard Time', text: '(GMT+13:00) Nuku\'alofa' },
  { value: 'Azores Standard Time', text: '(GMT-01:00) Azores' },
  { value: 'Cape Verde Standard Time', text: '(GMT-01:00) Cape Verde Is.' },
  { value: 'Mid-Atlantic Standard Time', text: '(GMT-02:00) Mid-Atlantic' },
  { value: 'E. South America Standard Time', text: '(GMT-03:00) Brasilia' },
  { value: 'Argentina Standard Time', text: '(GMT-03:00) Buenos Aires' },
  { value: 'SA Eastern Standard Time', text: '(GMT-03:00) Georgetown' },
  { value: 'Greenland Standard Time', text: '(GMT-03:00) Greenland' },
  { value: 'Montevideo Standard Time', text: '(GMT-03:00) Montevideo' },
  { value: 'Newfoundland Standard Time', text: '(GMT-03:30) Newfoundland' },
  { value: 'Atlantic Standard Time', text: '(GMT-04:00) Atlantic Time (Canada)' },
  { value: 'SA Western Standard Time', text: '(GMT-04:00) La Paz' },
  { value: 'Central Brazilian Standard Time', text: '(GMT-04:00) Manaus' },
  { value: 'Pacific SA Standard Time', text: '(GMT-04:00) Santiago' },
  { value: 'Venezuela Standard Time', text: '(GMT-04:30) Caracas' },
  { value: 'SA Pacific Standard Time', text: '(GMT-05:00) Bogota, Lima, Quito, Rio Branco' },
  { value: 'Eastern Standard Time', text: '(GMT-05:00) Eastern Time (US & Canada)' },
  { value: 'US Eastern Standard Time', text: '(GMT-05:00) Indiana (East)' },
  { value: 'Central America Standard Time', text: '(GMT-06:00) Central America' },
  { value: 'Central Standard Time', text: '(GMT-06:00) Central Time (US & Canada)' },
  { value: 'Central Standard Time (Mexico)', text: '(GMT-06:00) Guadalajara, Mexico City, Monterrey' },
  { value: 'Canada Central Standard Time', text: '(GMT-06:00) Saskatchewan' },
  { value: 'US Mountain Standard Time', text: '(GMT-07:00) Arizona' },
  { value: 'Mountain Standard Time (Mexico)', text: '(GMT-07:00) Chihuahua, La Paz, Mazatlan' },
  { value: 'Mountain Standard Time', text: '(GMT-07:00) Mountain Time (US & Canada)' },
  { value: 'Pacific Standard Time', text: '(GMT-08:00) Pacific Time (US & Canada)' },
  { value: 'Pacific Standard Time (Mexico)', text: '(GMT-08:00) Tijuana, Baja California' },
  { value: 'Alaskan Standard Time', text: '(GMT-09:00) Alaska' },
  { value: 'Hawaiian Standard Time', text: '(GMT-10:00) Hawaii' },
  { value: 'Samoa Standard Time', text: '(GMT-11:00) Midway Island, Samoa' },
  { value: 'Dateline Standard Time', text: '(GMT-12:00) International Date Line West' },
]
