namespace DevWinUI;

public enum CryptoMode
{
    Encrypt,
    Decrypt
}
public enum RSAKeySize : uint
{
    RSA512 = 512,
    RSA1024 = 1024,
    RSA2048 = 2048,
    RSA4096 = 4096,
}
public enum EncodeType
{
    Hex,
    Base64
}

public enum HashAlgorithm
{
    MD5,
    SHA1,
    SHA256,
    SHA384,
    SHA512
}

public enum AsymmetricAlgorithm
{
    DSA_SHA1,
    DSA_SHA256,
    ECDSA_P256_SHA256,
    ECDSA_P384_SHA384,
    ECDSA_P521_SHA512,
    RSA_OAEP_SHA1,
    RSA_OAEP_SHA256,
    RSA_OAEP_SHA384,
    RSA_OAEP_SHA512,
    RSA_PKCS1,
    RSASIGN_PKCS1_SHA1,
    RSASIGN_PKCS1_SHA256,
    RSASIGN_PKCS1_SHA384,
    RSASIGN_PKCS1_SHA512,
    RSASIGN_PSS_SHA1,
    RSASIGN_PSS_SHA256,
    RSASIGN_PSS_SHA384,
    RSASIGN_PSS_SHA512,
    ECDSA_SHA256,
    ECDSA_SHA384,
    ECDSA_SHA512
}
