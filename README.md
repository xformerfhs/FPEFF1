# FPEFF1
Visual Basic .Net class for the format preserving encryption algorithm FF1 as specified in [NIST Special Publication 800-38G (March 2016)](http://nvlpubs.nist.gov/nistpubs/SpecialPublications/NIST.SP.800-38G.pdf "NIST SP 800-38G").

This project contains a shared class "FF1" that implements the algorithm mentioned above. Additionally it contains a simple command line program to interface to the FF1 class. There are also various unit tests included.

The command line program can be used like this:

    fpeff1 encrypt 10 0,1,2,3,4,5,6,7,8,9 2B7E151628AED2A6ABF7158809CF4F3C 39383736353433323130    

This means: Encrypt the numbers "0,1,2,3,4,5,6,7,8,9" with the FF1 algorithm using the key "2B7E151628AED2A6ABF7158809CF4F3C" and the tweak "39383736353433323130". The expected output is:

    6,1,2,4,2,0,0,7,7,3

The decryption would be

    fpeff1 decrypt 10 6,1,2,4,2,0,0,7,7,3 2B7E151628AED2A6ABF7158809CF4F3C 39383736353433323130
    
which should yield

    0,1,2,3,4,5,6,7,8,9
    
## Contributing
Feel free to submit a pull request with new features, improvements on tests or documentation and bug fixes.

## Contact
Frank Schwab ([Mail](mailto:frank.schwab@deutschebahn.com "Mail"))

## License
FPEFF1 is released under the 3-clause BSD license. See "LICENSE" for details.
