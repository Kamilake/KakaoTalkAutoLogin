# KakaoTalkAutoLogin

여러 PC에서 카카오톡 자동 로그인


https://user-images.githubusercontent.com/15166740/225289222-b16525f1-9239-4537-9c79-9414aeaa191d.mp4


## credentials.json

```json
{
  "kakaoAccount" : "exjang0@gmail.com",
  "password" : "echo MyPasswordHere | openssl enc -aes-256-cbc -a -salt -pass pass:kamikami -iter 100"
}
```

## 이게 안전한지 어떻게 알고 암호를 막 쳐요?

그러게요! 소스에서 빌드하세요!
