using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface Protocol<T> {
    void msg2Bytes(T t);
    T bytes2Msg(byte[] content);
}