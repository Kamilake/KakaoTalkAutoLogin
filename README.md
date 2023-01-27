# KakaoTalkAutoLogin

여러 PC에서 카카오톡 자동 로그인

## credentials.json

```json
{
  "kakaoAccount" : "exjang0@gmail.com",
  "password" : "echo MyPasswordHere | openssl enc -aes-256-cbc -a -salt -pass pass:kamikami -iter 100"
}
```

## 이게 안전한지 어떻게 알고 암호를 막 쳐요?

그러게 말입니다 제발 소스에서 빌드해서 사용하도록 합시다.  
(물론 당연히 백도어는 없습니다)
