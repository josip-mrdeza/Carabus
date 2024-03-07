namespace Carabus.LowLevel;

/// <summary>
/// Four 32-bit data registers are used for arithmetic, logical, and other operations. These 32-bit registers can be used in three ways. <br/>
/// Lower halves of the 32-bit registers can be used as four 16-bit data registers: AX, BX, CX and DX.  <br/>
/// Lower and higher halves of the above-mentioned four 16-bit registers can be used as eight 8-bit data registers: AH, AL, BH, BL, CH, CL, DH, and DL.
/// <![CDATA[https://www.tutorialspoint.com/assembly_programming/assembly_registers.htm]]>
/// </summary>
public enum AsmRegister32bit
{
    eax,
    ebx,
    ecx,
    edx
}

public enum AsmRegister16bit
{
    /// <summary>
    /// Accumulator <br/>
    /// AX is the primary accumulator; it is used in input/output and most arithmetic instructions.
    /// For example, in multiplication operation, one operand is stored in EAX or AX or AL register according to the size of the operand.
    /// </summary>
    ax,
    /// <summary>
    /// Base <br/>
    /// BX is known as the base register, as it could be used in indexed addressing.
    /// </summary>
    bx,
    /// <summary>
    /// Counter <br/>
    /// CX is known as the count register, as the ECX, CX registers store the loop count in iterative operations.
    /// </summary>
    cx,
    /// <summary>
    /// Data <br/>
    /// DX is known as the data register. It is also used in input/output operations.
    /// It is also used with AX register along with DX for multiply and divide operations involving large values.
    /// </summary>
    dx
}

public enum AsmRegister8bit
{
    ah,
    bh,
    ch,
    dh
}

public enum AsmPointerRegister
{
    /// <summary>
    /// Instruction Pointer (IP) − The 16-bit IP register stores the offset address of the next instruction to be executed.<br/>
    /// IP in association with the CS register (as CS:IP) gives the complete address of the current instruction in the code segment.
    /// </summary>
    ip,
    /// <summary>
    /// Stack Pointer (SP) − The 16-bit SP register provides the offset value within the program stack.  <br/>
    /// SP in association with the SS register (SS:SP) refers to be current position of data or address within the program stack.
    /// </summary>
    sp,
    /// <summary>
    /// Base Pointer (BP) − The 16-bit BP register mainly helps in referencing the parameter variables passed to a subroutine. <br/>
    /// The address in SS register is combined with the offset in BP to get the location of the parameter.<br/>
    /// BP can also be combined with DI and SI as base register for special addressing.
    /// </summary>
    bp
}

public enum AsmIndexRegister
{
    /// <summary>
    /// Source Index (SI) − It is used as source index for string operations.
    /// </summary>
    si,
    /// <summary>
    /// Destination Index (DI) − It is used as destination index for string operations.
    /// </summary>
    di
}