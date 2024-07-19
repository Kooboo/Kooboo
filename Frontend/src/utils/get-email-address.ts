// 获取邮箱地址
export function getEmailAddress(address: string) {
  const reg = /\<(\S*)\>/;
  //通过当address含有'< >'时，取'< >'里的值判断
  if (address.match(reg)) {
    return address.match(reg)![1];
  } else {
    return address;
  }
}
