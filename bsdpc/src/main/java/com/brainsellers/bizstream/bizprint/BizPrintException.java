package com.brainsellers.bizstream.bizprint;

public class BizPrintException extends Exception {

    public BizPrintException(final String message) {
        super(message);
    }

    public BizPrintException(final String message, final Throwable cause) {
        super(message, cause);
    }
}
